# D365 Commerce CSU Performance Test

A Dynamics 365 Commerce performance testing framework that measures and tracks the execution time of critical sales workflows in the Commerce Runtime (CRT).

## Project Overview

This project provides a specialized API endpoint for the Dynamics 365 Commerce Runtime that enables performance testing of complete sale workflows. It measures the performance of individual operations (such as creating carts, adding items, applying payments, and checking out) and provides detailed timing metrics for each step in the workflow.

The project targets the **Dynamics 365 Commerce version 9.58+** and is designed to be deployed as a Commerce Runtime extension.

## Key Features

- **Workflow Performance Measurement**: Executes complete sale transactions while capturing execution times for each operation
- **Granular Metrics**: Provides detailed performance data for:
  - Cart creation
  - Customer attachment
  - Adding cart lines/items
  - Applying payments
  - Cart checkout
- **HTTP API Endpoint**: RESTful API endpoint (`/PerformanceTests/PTWorkflowExecute`) for triggering performance test workflows
- **Transaction Support**: Full integration with Dynamics 365 Commerce sales order processing
- **Customizable Parameters**: Support for channel, store, terminal, staff, and customer account configuration

## Project Structure

### Core Components

- **Controllers** (`Controllers/`)
  - `PTWorkflowController.cs`: REST API controller that exposes the performance testing endpoint

- **Workflows** (`Workflows/`)
  - `PTSaleWorkflow.cs`: Core workflow logic that orchestrates the complete sale transaction and captures performance metrics

- **Messages** (`Messages/`)
  - `PTWorkflowRequest.cs`: Internal CRT request message
  - `PTWorkflowResponse.cs`: CRT response message with workflow results and metrics

- **Contracts** (`Contracts/`)
  - `PTWorkflowExecuteRequest.cs`: API request payload contract
  - `PTWorkflowPerformance.cs`: Performance metrics data structure
  - `PTWorkflowMeasurement.cs`: Individual operation timing data
  - `PTWorkflowLine.cs`: Sales line item definition
  - `PTWorkflowSaleLineResult.cs`: Sale line result data
  - Other supporting contracts

- **Execution** (`Execution/`)
  - `ICommerceRequestExecutor.cs`: Interface for executing CRT requests
  - `RequestContextRequestExecutor.cs`: Implementation for executing requests through the CRT pipeline

- **Request Handlers** (`RequestHandlers/`)
  - `PTWorkflowRequestHandler.cs`: CRT request handler that processes `PTWorkflowRequest` messages

## Technical Details

### Framework & Dependencies

- **Target Framework**: `.NET Standard 2.0`
- **Primary Dependency**: `Microsoft.Dynamics.Commerce.Sdk.Runtime` (v9.58.x)
- **Build System**: .NET SDK project format

### Architecture

The project implements the Dynamics 365 Commerce Runtime extensibility pattern:

1. **HTTP Controller Layer**: Accepts incoming HTTP requests via the REST API
2. **Message/Request Layer**: Converts HTTP payloads into CRT request messages
3. **Request Handler Layer**: Processes requests through the CRT pipeline
4. **Workflow Layer**: Executes the business logic with performance instrumentation
5. **Execution Layer**: Handles actual CRT operations (cart management, payments, etc.)

Performance metrics are captured using `System.Diagnostics.Stopwatch` and returned as part of the workflow response.

### API Endpoint

**POST** `/PerformanceTests/PTWorkflowExecute`

**Authorization Required**: Device, Employee, or Application roles

**Request Body** (`PTWorkflowExecuteRequest`):
- `WorkflowRequestId`: Unique identifier for the test run
- `channelId`: Retail channel identifier
- `storeId`: Store identifier
- `terminalId`: Terminal identifier
- `staffId`: Staff member identifier
- `CustomerAccountNumber`: (Optional) Customer account number
- `Lines`: Collection of sale line items

**Response** (`PTWorkflowResponse`):
- `WorkflowRequestId`: Echo of request ID
- `CartId`: Generated cart identifier
- `TransactionId`: Transaction identifier
- `SalesId`: Sales order identifier
- Performance metrics for each workflow step

## Getting Started

### Requirements

- Visual Studio 2022
- .NET 6.0 SDK or later (for building)
- Dynamics 365 Commerce Runtime v9.58 or later (for deployment)

### Building

1. Clone the repository:
   ```bash
   git clone https://github.com/sergeiko/D365CommerceCSUPerformanceTest.git
   cd D365CommerceCSUPerformanceTest
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

### Deployment

This is a Commerce Runtime extension project. The compiled assembly should be deployed to the Commerce Scale Unit (CSU) according to Dynamics 365 Commerce deployment procedures:

1. Package the compiled DLL as a Commerce Runtime extension
2. Deploy to your CSU environment following Microsoft's extension deployment guidelines
3. Restart the CSU to load the new extension

## Usage

Once deployed, you can invoke the performance testing workflow via HTTP POST:

```http
POST https://<your-csu-instance>/PerformanceTests/PTWorkflowExecute
Authorization: Bearer <auth-token>
Content-Type: application/json

{
  "WorkflowRequestId": "TEST-001",
  "channelId": 1,
  "storeId": "HOUSTON",
  "terminalId": "POS001",
  "staffid": "000001",
  "CustomerAccountNumber": "CUST-001",
  "Lines": [
    {
      "ItemId": "0001",
      "Quantity": 1,
      "Price": 99.99
    }
  ]
}
```

The response will include performance measurements for each step of the workflow, allowing you to analyze transaction processing performance.

## Use Cases

- Performance baseline establishment for sales transactions
- Regression testing to detect performance degradation in updates
- Benchmarking different configurations or customizations
- Load testing and stress testing with multiple concurrent requests
- Identifying performance bottlenecks in the commerce runtime

## Code Style & Contributing

- This repository follows C# coding standards and conventions
- XML documentation comments are used throughout the codebase
- Follow the existing project structure and naming conventions when contributing

## Version

**Version**: 9.58.0.0 (Aligned with Dynamics 365 Commerce v9.58)

## License

Specify the license for this project.

## Additional Resources

- [Dynamics 365 Commerce Documentation](https://docs.microsoft.com/en-us/dynamics365/commerce/)
- [Commerce Runtime Extensions](https://docs.microsoft.com/en-us/dynamics365/commerce/dev-itpro/crt-extensions)
- Repository: [D365CommerceCSUPerformanceTest](https://github.com/sergeiko/D365CommerceCSUPerformanceTest)

## Support & Contact

For questions or issues related to this performance testing framework, please open an issue on the GitHub repository.
