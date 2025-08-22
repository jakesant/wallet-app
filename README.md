# wallet-app

A wallet application built with .NET 9.0. This app has a background job running every minute to fetch exchange rates from the European Central Bank (ECB) and store them in a SQL Server database. The application provides an API to manage wallets, allowing users to create wallets, retrieve wallet balances, and update those balances with optional currency conversion.

---

## 📂 Solution Structure

- **Wallet.Demo**  
  Actually an API :) contains the WalletController, and background job for fetching exchange rates.  

- **Wallet.Domain**  
  Holds entities and enums. Basically the pure domain layer with no dependencies on infrastructure.

- **Wallet.Gateway**  
  Contains service logic, DTOs, models, strategies, and interfaces. This is where the wallet business logic lives (`WalletService`, strategies for balance changes, ECB client, etc.).  

- **Wallet.Infrastructure**  
  Deals with data access and persistence. Contains EF Core DbContext, migrations, repositories, and entity configurations.  

- **Wallet.Tests**  
  Unit and integration tests for controllers and service classes.

## 🚀 Getting Started
1. Set Wallet.Demo as the startup project.
2. Ensure you have a SQL Server instance running and update the connection string in `appsettings.json` if necessary.
3. Run the application. It will automatically apply the latest migrations to the DB.
4. Test in Swagger or Postman