# TestMongoDBConnection

This project demonstrates how to connect to both a local and cloud MongoDB instance using C#. It includes functionality to test connections, read documents from collections, and log results.

## Features

- Connect to local MongoDB and cloud MongoDB Atlas.
- Ping the MongoDB server to check connectivity.
- Read documents from specified collections.
- Log connection results to a file.

## Prerequisites

- .NET Framework
- MongoDB.Driver NuGet package

## Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/maheshdharhari/TestMongoDBConnection.git
   cd TestMongoDBConnection
   ```

2. Update the following constants in `Program.cs`:
   - `Host`: Your MongoDB host (default: `localhost`).
   - `DatabaseName`: Your local database name (default: `MaheshDataBase`).
   - `CloudDatabaseName`: Your cloud database name (default: `sample_mflix`).
   - `UserName` and `Password`: Your MongoDB credentials.
   - `ConnectionUri`: Your MongoDB Atlas connection string.

## Usage

Run the application in Visual Studio or via command line:

```bash
msbuild /t:Run
```


The application will:
- Test the local MongoDB connection.
- Read and log documents from the local and cloud collections.
- Output the log to a file named `Support.Log.<timestamp>.txt`.

## License

This project is licensed under the MIT License.

```

Let me know if you need any further adjustments!
