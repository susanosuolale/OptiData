<div align="center">
  <h1>🚀 OptiData</h1>
  <h3>The Intelligent Data Bundle Optimizer</h3>
  <p>An enterprise-grade ASP.NET Core web application demonstrating Clean Architecture, CQRS, Machine Learning, and Real-Time WebSockets.</p>
</div>

---

## 📖 The Problem: "The Data Guessing Game"
Consumers are constantly forced to guess their data usage. Buying 3.2GB for 2 days is too little, leading to expensive pay-as-you-go overages. Buying 5GB for 1 day is too much, leading to financial waste when the data expires. OptiData was engineered to permanently eliminate this guessing game.

## 💡 The Solution
OptiData uses a custom **Machine Learning model (ML.NET)** to analyze a user's historical consumption footprint. It dynamically predicts exactly how much data they will need for a given timeframe, calculates the mathematically cheapest combination of data bundles to fulfill that need, and utilizes a **disconnected background worker (Hangfire)** to seamlessly auto-purchase the data just before the user runs out.

---

## 🛠 Enterprise Architecture & Technologies

This project was built to showcase senior-level backend engineering principles:

* **Framework**: ASP.NET Core 10 MVC
* **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Presentation)
* **Design Patterns**: CQRS (Command Query Responsibility Segregation) via **MediatR**
* **Database**: PostgreSQL with Entity Framework Core (Code-First Auto-Migrations)
* **Real-Time Communication**: SignalR (WebSockets)
* **Background Jobs**: Hangfire (Persistent PostgreSQL Storage)
* **Machine Learning**: Microsoft ML.NET
* **Integrations**: Paystack API (Payment Mock), OpenAI API (Intelligent AI Assistant)
* **CI/CD**: Fully automated deployment to Render Cloud
* **Frontend**: HTML5, CSS3, JavaScript (with dynamic micro-animations)

---

## ✨ Core Features & Technical Highlights

### 1. CQRS and Highly Decoupled Logic
The application strictly separates Read and Write operations using **MediatR**. The `OptimizeBundlesCommand` handles the complex logic of calling the ML prediction service, querying multiple telecom provider endpoints, and mathematically determining the absolute cheapest bundle combination—keeping the Controllers completely clean and scalable.

### 2. Hyper-Personalized Machine Learning (ML.NET)
Instead of relying on generic averages, the system trains a local ML model on the user's specific SQL data history to predict their exact consumption curve for any given timeframe, ensuring zero financial waste.

### 3. Just-In-Time Background Automation (Hangfire)
When a user enables "Auto-Purchase", OptiData schedules a background job using **Hangfire**. 
* **Zero-Interruption**: The background worker securely executes a simulated REST API call to Paystack to buy the data.
* **Just-In-Time Execution**: The job waits until exactly 1 hour before the user is mathematically predicted to run out of data.
* **Idempotency & Safety**: Before executing the purchase, the job queries the database to check the user's `CurrentBalanceMB`. If the user manually recharged early, the job gracefully aborts to prevent double-charging.

### 4. Real-Time Event-Driven UI (SignalR)
To provide a seamless user experience, the disconnected Hangfire background job communicates directly with the frontend. Upon a successful automated purchase, it broadcasts a **SignalR WebSocket** event, instantly popping up a Toast Notification on the user's screen without requiring a page refresh.

### 5. AI Assistant Integration
Integrated the **OpenAI API** to provide an intelligent, context-aware chatbot on the frontend that can explain complex data predictions and bundle mathematics to the user in plain English.

---

## 🚀 Live Demo & Deployment

The application is fully containerized and automatically deployed via a CI/CD pipeline. 
**Live Application**: *(Replace with Render Link)*

### How to test the Background Automation instantly:
1. Open the UI and select a duration of **1 Hour**.
2. Because the Hangfire job is programmed to purchase data exactly 1 hour *before* expiration, the mathematical wait time becomes `0` hours.
3. Click "Enable Auto-Purchase".
4. The background job will execute **instantly**, triggering the Paystack simulation and firing the real-time SignalR Toast Notification straight to your browser!

---
*Built by [Your Name] — Open to new opportunities as a .NET / C# Developer.*
