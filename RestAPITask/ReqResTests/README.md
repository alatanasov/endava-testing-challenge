# 🌐 ReqRes API Automation Suite

 Automated REST API test suite for the ReqRes
 public API — developed as part of the Endava Testing Challenge.
This project validates CRUD operations and error handling across multiple endpoints.

## ⚙️ Tech Stack:

- C# / .NET 9 -> Programming language & runtime
- NUnit -> Test framework
- RestSharp -> HTTP Client
- Newtonsoft.Json -> Config parsing
- Fluent Assertions -> Assertion Tool

## 📁 Project Structure:

<img width="1058" height="447" alt="image" src="https://github.com/user-attachments/assets/3ea02733-349c-4e5e-bece-68ecd76d98bf" />

## 🧩 Configuration:

```{
  "ApiBaseUrl": "https://reqres.in",
  "ApiKey": "reqres-free-v1",
  
  "Environments": {
    "Dev": "https://reqres.in",
    "Testing": "https://reqres.in",
    "Staging": "https://reqres.in"
  }
}
```

## 🚀 Running the Tests (CMD)

Runt the test with the default parameters :

``dotnet test``

Run on specific environment (Dev, Testing, Staging) : 

```dotnet test -- "TestRunParameters.Parameter(name=\"env\",value=\"Dev\")"```

## 🧷 Running with Environment Variables (Windows CMD)

You can also control the run using environment variables (read by BaseTest):

- TEST_ENVIRONMENT → Dev | Testing | Staging

Example: 

setx TEST_ENVIRONMENT Testing

## 🧠 Test Scenarios

| 🧪 **Test Name**                                 | 🏷️ **Category**    | 🧩 **Module / Area**           | 🎯 **Purpose / Description**                                                                                                     | 🕒 **Priority** |
| ------------------------------------------------ | ------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------- | --------------- |
| `VerifyAllUsersCanBeRetrievedSuccessfully`       | `Users.List`        | **GET** → `/api/users?page=1`  | Retrieves a paginated list of users, validates JSON structure, and ensures all fields (ID, name, email) are correctly populated. | 🔴 High         |
| `VerifySingleUserDetailsAreReturnedCorrectly`    | `Users.Single`      | **GET** → `/api/users/{id}`    | Fetches details of a specific user and verifies correctness of ID, name, email, and avatar in the response.                      | 🔴 High         |
| `VerifyRequestForNonExistingUserReturnsNotFound` | `Users.NonExisting` | **GET** → `/api/users/{id}`    | Sends a request for a non-existent user ID and verifies the API returns a **404 Not Found** with an empty JSON body.             | 🟡 Medium       |
| `VerifyNewUserIsCreatedSuccessfully`             | `Users.Create`      | **POST** → `/api/users`        | Creates a new user with unique data and validates name, job title, and creation timestamp in the response.                       | 🔴 High         |
| `VerifyUserIsDeletedSuccessfully`                | `Users.Delete`      | **DELETE** → `/api/users/{id}` | Deletes an existing user, expects **204 No Content**, and confirms the user cannot be retrieved afterward.                       | 🟢 Low          |



