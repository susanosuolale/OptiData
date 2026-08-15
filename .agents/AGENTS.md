<RULE[aesthetic_preference]>
## Classy Enterprise Aesthetic
- **Visual Design**: The UI must ALWAYS be designed with a "classy", professional, and sleek enterprise aesthetic.
- **Color Palette**: Use deep, sophisticated color palettes (e.g., dark modes, slate grays, muted blues, or monochrome with sleek accents). Do not use generic Bootstrap colors or overly bright pinks.
- **Micro-interactions**: Include smooth, subtle micro-animations and hover effects that fit a highly premium, modern enterprise application.
- **CSS**: Apply these styles whenever generating or updating HTML/CSS views.
</RULE[aesthetic_preference]>

<RULE[portfolio_goal]>
## Ultimate Goal: The 100% Interview Portfolio
- **The Only Metric**: Every architectural decision, feature, and code commit must serve the sole purpose of securing a C# Developer interview.
- **Showcase Enterprise C#**: We must "over-engineer" the backend to demonstrate senior-level enterprise skills (Microservices, Event Sourcing, Real-time WebSockets, Background Workers, CI/CD, ML.NET).
</RULE[portfolio_goal]>

<RULE[repository_protection]>
## Strict Code Preservation (AssetDesk)
- **Do Not Touch**: The `AssetDesk` project is a read-only reference repository. Never delete, modify, or refactor any code or comments inside it. 
- **New Projects Only**: All new portfolio work must be scaffolded in completely separate, brand new directories.
</RULE[repository_protection]>

<RULE[project_domain]>
## Domain: Data Bundle Optimizer (OptiData)
- **The Problem ("The Data Guessing Game")**: Consumers are forced to guess their data usage. Buying 3.2GB for 2 days is too little, but 4.5GB for 1 day is too much. This guessing game leads to constant financial waste through expired data or expensive pay-as-you-go overages.
- **The Solution**: An intelligent application that ingests historical usage data, predicts exactly how much data is needed for a user-selected timeframe (Day, Month, Year), and recommends the mathematically optimal combination of bundles to purchase.
- **Advanced Features for Employers**: 
  - **Machine Learning (ML.NET)**: Predicting future usage based on historical trends.
  - **Background Automation (Hangfire)**: Automatically simulating the purchase of micro-bundles just before the user runs out to prevent interruptions.
</RULE[project_domain]>

<RULE[senior_guidance_pacing]>
## Senior Leadership with Step-by-Step Pacing
- **Lead the Project (Never Ask What's Next)**: Do NOT ask the user "what should we do next?". The agent must autonomously decide the next logical technical step from beginning to end, ensuring the most optimized, interview-ready enterprise code (like CQRS) is used.
- **Maintain Strict Explanation Style**: Even while leading, the agent MUST strictly adhere to the user's learning style. 
  - **Code First**: Always implement the code for the single next step FIRST, before explaining what it does.
  - Explain the exact code for the single next step.
  - Use basic English and literal descriptions.
  - NEVER use analogies.
  - Wait for explicit confirmation that the user understands the current code step before writing the next piece.
</RULE[senior_guidance_pacing]>

<RULE[clean_csharp_imports]>
## Clean C# Imports
- **Use Using Statements**: Never use fully qualified namespaces inline for types (e.g., do not write `OptiData.Domain.Enums.DataProvider`).
- **Top of File**: Always add the appropriate `using` statement (e.g., `using OptiData.Domain.Enums;`) at the top of the file and use the short type name (`DataProvider`) inline.
- **Dependency Injection Setup**: This rule applies strictly to all files, including `Program.cs` and any service registration extensions. Never use fully qualified namespaces inside `builder.Services` configurations.
</RULE[clean_csharp_imports]>

<RULE[proactive_user_guidance]>
## Proactive User Guidance & UX
- **Carry Users Along**: Always proactively design the UI/UX to be as informative and self-explanatory as possible.
- **Prevent Confusion**: Anticipate edge cases, algorithm decisions, or background logic and present them clearly in the UI so the user is never lost or confused.
- **Do Not Wait for Prompts**: Implement these informative UI features automatically during development without waiting for the user to explicitly ask for them.
</RULE[proactive_user_guidance]>
