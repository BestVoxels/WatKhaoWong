# 🏛️ Wat Khao Wong — Digital Community Platform

A production-grade mobile application architected for a non-profit organization.
This project digitizes community engagement, activity tracking, and temple management.
Unlike a standard "app," this is a full-stack solution serving **1,700+ organic users** with complex backend requirements, real-time data synchronization, and secure authentication.

I independently managed the entire Software Development Life Cycle (SDLC), from requirement gathering and system architecture to deployment on both the **Apple App Store** and **Google Play Store**.

---

## 🏗️ Architecture Overview

This project demonstrates a **Serverless Architecture** optimized for high-performance mobile clients.

### **1. Backend Engineering (Firebase Realtime Database)**
* **O(1) Read Complexity:** Designed a **flattened JSON data structure** to avoid deep nesting. This ensures that reading the "Leaderboard" does not require downloading the entire user database, allowing for **O(1) direct access** and minimal bandwidth usage.
* **Split-Node Logic:** Architected the database to split each "Leaderboard Category" from "Users Profile" data, ensuring that heavy read operations (like loading the leaderboard) are decoupled from heavy write operations (like updating a profile).
* **Server-Side Time Validation (Anti-Cheat):** Implemented strict **UTC / Gregorian Calendar synchronization** to validate all user activities. This prevents client-side exploitation where users manipulate their device time to "cheat" in time-sensitive challenges.

### **2. Authentication & Security**
* **Multi-Provider Login:** Integrated **Firebase Authentication** supporting both **Email/Password** and **Phone Number (OTP)** login flows.
* **Persistent User Sessions:** Managed secure token states to keep users logged in across sessions while ensuring data privacy via strict database security rules (Rules language).

### **3. Asynchronous Performance & UI**
* **Non-Blocking UI Pipeline:** Engineered a responsive UI utilizing **C# Async/Await** patterns. This distributes heavy tasks (like instantiating 100+ leaderboard rows) across multiple frames, preventing the main thread from freezing.
* **120Hz Optimization:** Optimized the rendering loop and canvas scalers to support **High Refresh Rate (120Hz)** devices, ensuring fluid scrolling and interaction.
* **Object Pooling:** Implemented custom pooling systems for UI list items to eliminate memory allocation overhead during UI instantiating.

---

## 📱 System Showcase & Engineering Highlights

Below are the core modules of the application, demonstrating the full software development lifecycle from architecture to production.

### **1. Production Deployment & SDLC**
<img width="1434" height="1189" alt="Image" src="https://github.com/user-attachments/assets/46a042aa-4493-4d92-bafc-eb1f56ee5914" />

**Full Lifecycle Management**
I independently managed the build pipeline, compliance, and App Store Optimization (ASO) for both iOS and Android. The app maintains a **99.8% crash-free session rate** in production.

---

### **2. Scalable Real-Time Leaderboard Architecture**
<img width="330" height="717" alt="Image" src="https://github.com/user-attachments/assets/11e70c4a-e8f0-4482-8ced-b12308dd58e6" />

**Optimized Data Retrieval**
The leaderboard system does not download the entire user list. It queries specific, flattened data nodes.
Refactored data retrieval logic to merge existential checks with fetching, reducing redundant API calls by 50%
- **Performance:** Rendering handles 100+ rows without frame drops due to Object Pooling and Async instantiation.

- **All-Time Rankings:** Persistent historical data.
- **Daily Resets:** Automated logic to reset and archive daily scores.
- **Challenge Mode:** Temporary, admin-controlled event rankings.

---

### **3. Challenge & Event System**
<img width="330" height="717" alt="Image" src="https://github.com/user-attachments/assets/d263bdb2-9b6c-4051-a510-df4de5baa19f" />

**Server-Authoritative Logic**
A "Live Ops" feature allowing administrators to push new challenges to clients in real-time.
- **State Management:** Clients listen for challenge states (Pending / Active / Ended) and update the UI dynamically without requiring an app update.
- **Event-Driven:** Uses Observer patterns to trigger UI changes when a new challenge goes live.

---

### **4. Secure Authentication & Anti-Cheat Validation**
<img width="330" height="717" alt="Image" src="https://github.com/user-attachments/assets/6f2c0170-8c62-4942-bab6-13baa4de7f5b" />

- Integrated Firebase Authentication (Email/Password & Phone) with custom server-side logic.
- Implemented UTC/Gregorian time synchronization to validate client requests and prevent client-side data manipulation.

---

### **5. Prayer & Activity Tracking**
<img width="330" height="717" alt="Image" src="https://github.com/user-attachments/assets/23d5c116-60aa-4d02-8255-ab0114be6405" />

- **Digital Counter:** Tracks prayer rounds and meditation time with local persistence and cloud syncing.
- **Data Integrity:** syncs local progress with the cloud to prevent data loss even if the user switches devices.

---

### **6. Dynamic Localization System**
<img width="330" height="717" alt="Image" src="https://github.com/user-attachments/assets/8b966198-e13e-435a-97c3-a3b138bf5af4" />

- Developed a modular localization framework allowing real-time language switching (Thai/English) without scene reloading.

---

### **7. MVPR-Driven UI Architecture**
<img width="330" height="717" alt="Image" src="https://github.com/user-attachments/assets/bcace446-1399-46d4-afb9-02bee9a5d743" />

- Built using a custom Model-View-Presenter-Refined (MVPR) pattern to strictly decouple the UI layer from business logic. This architecture supports 120Hz refresh rates through efficient object pooling and asynchronous data handling.

---

### **8. End-to-End System Architecture & Dependency Mapping**
<img width="2891" height="1054" alt="Image" src="https://github.com/user-attachments/assets/07c0f53c-ff43-4b47-9c34-a72fd406f8cc" />

- A comprehensive visualization of the entire WatKhaoWong production codebase, illustrating the complex web of dependencies between hundreds of classes and modules.
- This diagram demonstrates the scale of the system and the successful application of a decoupled MVPR architecture, which was critical for managing complexity, ensuring maintainability, and enabling the integration of features like the real-time leaderboard and localization systems without creating spaghetti code.

---

## 📊 Measurable Impact

| Metric | Value | Technical Context |
| :--- | :--- | :--- |
| **Engagement** | **2h 22m** | Average daily session time per active user. |
| **Scale** | **1,779+** | Total organic installs across platforms. |
| **Performance** | **120Hz** | Optimized for high-refresh-rate displays. |

---

## 👤 Author
**Thanitsak Leuangsupornpong**
Software Engineer (Independent)
