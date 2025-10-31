# 🧪 SauceDemo UI Automation Suite
End-to-end automated UI testing suite for the [SauceDemo](https://www.saucedemo.com/)
 web application — developed as part of the Endava Testing Challenge.
This project verifies user workflows, including login, cart management, checkout, sorting, and logout across multiple browsers and screen resolutions.

## ⚙️ Tech Stack:

- C# / .NET 9 -> Programming language & runtime
- NUnit -> Test framework
- Selenium WebDriver -> Browser automation
- ExtentReports -> HTML reporting
- Newtonsoft.Json -> Config parsing
- Chrome / Firefox -> Supported browsers

## 📁 Project Structure:

<img width="848" height="505" alt="image" src="https://github.com/user-attachments/assets/5161dc23-c880-4b67-bca0-f7429e2804d1" />

## 🧩 Configuration:

```{
  "Credentials": {
    "StandardUser": "standard_user",
    "Password": "secret_sauce"
  },
  "Environments": {
    "Dev": "https://www.saucedemo.com/",
    "Testing": "https://www.saucedemo.com/",
    "Staging": "https://www.saucedemo.com/"
  },
  "Browsers": {
    "Default": "Chrome"
  },
  "Resolutions": {
    "Default": "1920x1080",
    "Tablet": "1024x768",
    "Mobile": "414x896"
  },
  "DefaultCheckout": {
    "FirstName": "Alexander",
    "LastName": "Atanasov",
    "PostalCode": "1336"
  }
}
```

## 🚀 Running the Tests (CMD)

Runt the test with the default parameters :

``dotnet test``

Run on specific environment (Dev, Testing, Staging) : 

```dotnet test -- "TestRunParameters.Parameter(name=\"env\",value=\"Dev\")"```

Run on a specific browser (Chrome or Firefox) :

```dotnet test -- "TestRunParameters.Parameter(name=\"browser\",value=\"Firefox\")"```

Run with environment + browser

```dotnet test -- "TestRunParameters.Parameter(name=\"env\",value=\"Testing\")" "TestRunParameters.Parameter(name=\"browser\",value=\"Firefox\")"```

Run with custom resolution (Desktop - 1920x1080, Tablet - 1024x768, Mobile - 414x896)

```dotnet test -- "TestRunParameters.Parameter(name=\"env\",value=\"Testing\")" "TestRunParameters.Parameter(name=\"browser\",value=\"Chrome\")" "TestRunParameters.Parameter(name=\"resolution\",value=\"1024x768\")"```

## 🧷 Running with Environment Variables (Windows CMD)

You can also control the run using environment variables (read by BaseTest):

- TEST_ENVIRONMENT → Dev | Testing | Staging
- TEST_BROWSER → Chrome | Firefox
- TEST_RESOLUTION → e.g., 1920x1080, 1024x768, 414x896

Example: 

setx TEST_ENVIRONMENT Testing

setx TEST_BROWSER Firefox

setx TEST_RESOLUTION 414x896


## 📊 Reporting

After each test run, a detailed HTML report is automatically generated using ExtentReports.

Location: ```\SauceDemoTests\TestResults```

## 🧠 Test Scenarios

| 🧪 **Test Name**                          | 🏷️ **Category**  | 🧩 **Module / Area**       | 🎯 **Purpose / Description**                                                                                                                                              | 🕒 **Priority** |
| ----------------------------------------- | ----------------- | -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------- |
| `VerifyCompleteOrderWithCartModification` | `CompletingOrder` | Checkout & Cart Management | Executes a complete purchase flow: logs in, adds and removes products, completes checkout, verifies successful order completion, and ensures the cart is empty afterward. | 🔴 High         |
| `VerifyItemsAreSortedByPriceHighToLow`    | `ItemsSorting`    | Inventory & Sorting        | Validates that the “Price (High → Low)” sorting option correctly orders all products by price in descending order on the inventory page.                                  | 🟢 Low          |






