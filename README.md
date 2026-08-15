# OptiData: The Intelligent Data Bundle Optimizer

OptiData is an enterprise-grade C# application designed to permanently solve the "Data Guessing Game". By utilizing Machine Learning and automated background workers, OptiData ensures you never run out of data unexpectedly while simultaneously ensuring you never waste money on data that expires.

## Architecture & Core Features

### 1. Hyper-Personalized Machine Learning Engine (ML.NET)
Most data applications blindly recommend bundles based on "average" consumer trends. OptiData's **ML.NET** engine trains on your unique, highly-specific historical dataset to predict your *exact* personalized data consumption curve.
* **Exact Volume Prediction**: You input a timeframe (e.g., 24 hours), and the ML model calculates the precise Megabytes you will consume, ensuring you never overpay for data that expires.
* **CQRS Optimization**: The `MediatR` pipeline takes this precise prediction and scans the database to find the absolute cheapest combination of telecom bundles to fulfill it mathematically.

### 2. The Just-In-Time Background Automation (Hangfire)
OptiData acts as an intelligent safety net. When you accept a prediction, you can enable "Auto-Purchase". 
* **Zero-Interruption**: Using **Hangfire**, the system schedules a disconnected background job to physically execute a REST API call to Paystack to buy the data.
* **Just-In-Time Execution**: The background job purposely waits until exactly 1 hour before you are mathematically predicted to run out of data. This guarantees your money stays safely in your bank account for as long as possible, but buys the data *just in time* so your internet never drops.

### 3. The Enterprise State-Check (Idempotency)
Background automation must be safe. When the Hangfire timer reaches zero, the very first thing it does is connect to the SQL database to check your `CurrentBalanceMB`. 
* If you manually bought data early, your balance will be high. The Hangfire job detects this and **gracefully aborts** to completely prevent double-charging.

### 4. Real-Time Event Driven UI (SignalR)
*Coming Soon: When the disconnected Hangfire job successfully charges the Paystack API, it pushes a real-time WebSocket notification directly to the user's browser, updating their balance instantly without a page refresh.*

## How to Demo in an Interview
To quickly prove the Hangfire background automation works during a 15-minute interview without waiting:
1. Open the UI and select a duration of **1 Hour**.
2. Because the job schedules exactly 1 hour *before* expiration, the mathematical wait time becomes exactly `0` hours.
3. Click "Enable Auto-Purchase", and the Hangfire job will execute **instantly** in the background, allowing you to show the successful Paystack transaction live!
