namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Workflows
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Execution;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    internal class PTSaleWorkflow
    {
        internal async Task<PTWorkflowResponse> RunAsync(
            ICommerceRequestExecutor executor,
            PTWorkflowRequest request,
            string cartId,
            Collection<PTWorkflowMeasurement> measurements)
        {
            ThrowIf.Null(executor, nameof(executor));
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(measurements, nameof(measurements));

            Cart cart = await RunMeasuredStepAsync(
                measurements,
                "CreateCart",
                () => this.CreateCartAsync(executor, request, cartId)).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(request.CustomerAccountNumber))
            {
                cart = await this.AddCustomerAsync(executor, cart, request.CustomerAccountNumber, measurements).ConfigureAwait(false);
            }

            cart = await this.AddCartLinesAsync(executor, request, cart, measurements).ConfigureAwait(false);
            cart = await this.AddCashPaymentAsync(executor, cart, measurements).ConfigureAwait(false);

            SalesOrder salesOrder = await RunMeasuredStepAsync(
                measurements,
                "CheckoutCart",
                () => this.CheckoutCartAsync(executor, cart)).ConfigureAwait(false);
            return CreateResponse(request, cartId, salesOrder);
        }

        private async Task<Cart> CreateCartAsync(
            ICommerceRequestExecutor executor,
            PTWorkflowRequest request,
            string cartId)
        {
            var cart = new Cart
            {
                Id = cartId,
                CartType = CartType.Shopping,
                ChannelId = request.ChannelId,
                TerminalId = request.TerminalId,
                StaffId = request.StaffId,
            };

            SaveCartResponse response = await executor.ExecuteAsync<SaveCartResponse>(
                new SaveCartRequest(cart, CalculationModes.All, false, TransactionOperationType.Create)).ConfigureAwait(false);
            return response.Cart;
        }

        private async Task<Cart> AddCustomerAsync(
            ICommerceRequestExecutor executor,
            Cart cart,
            string customerAccountNumber,
            Collection<PTWorkflowMeasurement> measurements)
        {
            GetCustomersServiceResponse customerResponse = await RunMeasuredStepAsync(
                measurements,
                "GetCustomer",
                () => executor.ExecuteAsync<GetCustomersServiceResponse>(
                    new GetCustomersServiceRequest(QueryResultSettings.SingleRecord, customerAccountNumber))).ConfigureAwait(false);

            if (customerResponse?.Customers?.Results == null || !customerResponse.Customers.Results.Any())
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Customer account '{0}' was not found.",
                    customerAccountNumber));
            }

            var cartWithCustomer = new Cart
            {
                Id = cart.Id,
                Version = cart.Version,
                CustomerId = customerAccountNumber,
            };

            SaveCartResponse response = await RunMeasuredStepAsync(
                measurements,
                "AddCustomerToCart",
                () => executor.ExecuteAsync<SaveCartResponse>(
                    new SaveCartRequest(cartWithCustomer, CalculationModes.All, false, TransactionOperationType.Update))).ConfigureAwait(false);
            return response.Cart;
        }

        private async Task<Cart> AddCartLinesAsync(
            ICommerceRequestExecutor executor,
            PTWorkflowRequest request,
            Cart cart,
            Collection<PTWorkflowMeasurement> measurements)
        {
            Cart current = cart;
            for (int index = 0; index < request.Lines.Count; index++)
            {
                PTWorkflowLine line = request.Lines[index];
                string stepName = string.Format(CultureInfo.InvariantCulture, "AddCartLine[{0}]", index + 1);
                CartLine cartLine = new CartLine
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    StaffId = request.StaffId,
                };

                SaveCartResponse response = await RunMeasuredStepAsync(
                    measurements,
                    stepName,
                    () => executor.ExecuteAsync<SaveCartResponse>(
                        new AddCartLinesRequest(current.Id, new[] { cartLine }, CalculationModes.All, current.Version))).ConfigureAwait(false);

                current = response.Cart;
            }

            return current;
        }

        private async Task<Cart> AddCashPaymentAsync(
            ICommerceRequestExecutor executor,
            Cart cart,
            Collection<PTWorkflowMeasurement> measurements)
        {
            string cashTenderTypeId = await RunMeasuredStepAsync(
                measurements,
                "GetCashTenderType",
                () => this.GetCashTenderTypeIdAsync(executor)).ConfigureAwait(false);

            decimal amount = await RunMeasuredStepAsync(
                measurements,
                "RoundCashAmount",
                () => this.GetRoundedCashAmountAsync(executor, cart.AmountDue, cashTenderTypeId)).ConfigureAwait(false);

            var tenderLine = new CartTenderLine
            {
                TenderTypeId = cashTenderTypeId,
                Amount = amount,
                AmountInTenderedCurrency = amount,
                CustomerId = cart.CustomerId,
            };

            var request = new SaveTenderLineRequest
            {
                CartId = cart.Id,
                CartVersion = cart.Version,
                CustomerAccountNumber = cart.CustomerId,
                TenderLine = tenderLine,
                OperationType = TenderLineOperationType.Create,
            };

            SaveTenderLineResponse response = await RunMeasuredStepAsync(
                measurements,
                "AddCashPayment",
                () => executor.ExecuteAsync<SaveTenderLineResponse>(request)).ConfigureAwait(false);
            return response.Cart;
        }

        private async Task<string> GetCashTenderTypeIdAsync(ICommerceRequestExecutor executor)
        {
            var request = new GetTenderTypeIdentifierServiceRequest(ExtensibleTransactionType.Sales, RetailOperation.PayCash);
            SingleEntityDataServiceResponse<string> response = await executor.ExecuteAsync<SingleEntityDataServiceResponse<string>>(request).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response?.Entity))
            {
                throw new InvalidOperationException("Cash tender type was not found for the channel.");
            }

            return response.Entity;
        }

        private async Task<decimal> GetRoundedCashAmountAsync(
            ICommerceRequestExecutor executor,
            decimal amount,
            string cashTenderTypeId)
        {
            GetRoundedValueServiceResponse response = await executor.ExecuteAsync<GetRoundedValueServiceResponse>(
                new GetPaymentRoundedValueServiceRequest(amount, cashTenderTypeId, false)).ConfigureAwait(false);
            return response.RoundedValue;
        }

        private async Task<SalesOrder> CheckoutCartAsync(
            ICommerceRequestExecutor executor,
            Cart cart)
        {
            CheckoutCartResponse response = await executor.ExecuteAsync<CheckoutCartResponse>(
                new CheckoutCartRequest(cart.Id, null, null, null, null, cart.Version, CheckoutLocation.Default)).ConfigureAwait(false);
            return response.SalesOrder;
        }

        private static async Task<T> RunMeasuredStepAsync<T>(
            Collection<PTWorkflowMeasurement> measurements,
            string name,
            Func<Task<T>> action)
        {
            DateTimeOffset startedUtc = DateTimeOffset.UtcNow;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var measurement = new PTWorkflowMeasurement
            {
                Name = name,
                StartedUtc = startedUtc,
                Status = "Success",
            };

            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                measurement.Status = "Failed";
                measurement.StatusMessage = exception.Message;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                measurement.EndedUtc = DateTimeOffset.UtcNow;
                measurement.DurationMilliseconds = stopwatch.ElapsedMilliseconds;
                measurements.Add(measurement);
            }
        }

        private static PTWorkflowResponse CreateResponse(
            PTWorkflowRequest request,
            string cartId,
            SalesOrder salesOrder)
        {
            Collection<PTWorkflowSaleLineResult> lineResults = new Collection<PTWorkflowSaleLineResult>(
                (salesOrder?.SalesLines ?? Enumerable.Empty<SalesLine>())
                    .Where(line => line != null && !line.IsVoided)
                    .Select(line => new PTWorkflowSaleLineResult
                    {
                        LineId = line.LineId,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        TotalAmount = line.TotalAmount,
                    })
                    .ToList());

            return new PTWorkflowResponse
            {
                WorkflowRequestId = request.WorkflowRequestId,
                CartId = cartId,
                TransactionId = salesOrder?.Id,
                SalesId = salesOrder?.SalesId,
                CustomerAccountNumber = request.CustomerAccountNumber,
                TotalItems = lineResults.Count,
                TotalQuantity = lineResults.Sum(line => line.Quantity),
                TotalSaleAmount = salesOrder?.TotalAmount ?? 0M,
                Currency = salesOrder?.CurrencyCode,
                Lines = lineResults,
            };
        }
    }
}
