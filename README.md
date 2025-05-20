# 📸 FlickrApp (.NET MAUI)

[![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512bd4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/maui/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![XAML](https://img.shields.io/badge/XAML-0C5384?style=for-the-badge&logoColor=white)](https://learn.microsoft.com/dotnet/maui/xaml)
[![JSON](https://img.shields.io/badge/JSON-000000?style=for-the-badge&logo=json&logoColor=white)](https://www.json.org/json-en.html)
[![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)](https://www.sqlite.org/index.html)
[![Flickr API](https://img.shields.io/badge/Flickr_API-0063dc?style=for-the-badge&logo=flickr&logoColor=white)](https://www.flickr.com/services/api/)
[![xUnit](https://img.shields.io/badge/xUnit-007ACC?style=for-the-badge&logo=.net&logoColor=white)](https://xunit.net/)
[![Platform](https://img.shields.io/badge/Platform-iOS%20%7C%20Android-lightgrey?style=for-the-badge)](https://learn.microsoft.com/dotnet/maui/)

---

## 📝 Description

FlickrApp is a sample project created to explore the capabilities of **.NET MAUI** for mobile cross-platform development and to demonstrate integration with external REST APIs, specifically the **Flickr API**.
It allows users to enter search terms and view a gallery of matching photos, with the ability to open a detailed view for each photo including title and author information.

---

## ✨ Key Features

- 🔍 **Advanced Photo Search**: Search Flickr photos by keyword using the official Flickr API.
- 🖼️ **Infinite Scrolling**: Dynamically load more images as the user scrolls.
- 📄 **Photo Detail Page**: Display selected photo with title, author, and other relevant info.
- 📱 **Clean & Mobile-Optimized UI**: User interface focused on a smooth mobile experience.
- 🧩 **MVVM Architecture**: Structured with the MVVM (Model-View-ViewModel) pattern for maintainability, testability, and separation of concerns.

---

## 💻 Technologies Used

| Technology                                                                 | Purpose                                       | Badge                                                                                                                                         |
|----------------------------------------------------------------------------|-----------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| [.NET MAUI](https://learn.microsoft.com/dotnet/maui/)                      | Cross-platform mobile app framework           | ![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512bd4?style=flat-square&logo=dotnet&logoColor=white)                                     |
| C#                                                                         | Programming language                          | ![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)                                                |
| XAML                                                                       | UI Markup Language for .NET MAUI              | ![XAML](https://img.shields.io/badge/XAML-0C5384?style=flat-square)                                                                          |
| [Flickr API](https://www.flickr.com/services/api/)                         | Fetching photos and metadata                  | ![Flickr API](https://img.shields.io/badge/Flickr_API-0063dc?style=flat-square&logo=flickr&logoColor=white)                                  |
| JSON                                                                       | Data interchange format (API, config)         | ![JSON](https://img.shields.io/badge/JSON-000000?style=flat-square&logo=json&logoColor=white)                                                 |
| SQLite                                                                     | Local data storage                            | ![SQLite](https://img.shields.io/badge/SQLite-07405E?style=flat-square&logo=sqlite&logoColor=white)                                          |
| [xUnit](https://xunit.net/)                                               | Unit Testing Framework for .NET               | ![xUnit](https://img.shields.io/badge/xUnit-007ACC?style=flat-square&logo=.net&logoColor=white)                                              |
| Git & GitHub                                                               | Version control and collaboration             | ![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github&logoColor=white)                                          |
| [JetBrains Rider](https://www.jetbrains.com/rider/)                        | Development environment (recommended)         | ![JetBrains Rider](https://img.shields.io/badge/JetBrains_Rider-000000?style=flat-square&logo=jetbrains&logoColor=white)                     |
| MVVM Pattern                                                               | Application architecture pattern              | ![MVVM](https://img.shields.io/badge/Architecture-MVVM-orange?style=flat-square)                                                              |

---

## 🚀 Getting Started

### Prerequisites

-   **[.NET SDK](https://dotnet.microsoft.com/en-us/download)**: Version 8.0 (or the latest LTS version).
-   **.NET MAUI Workload**:
    ```bash
    dotnet workload install maui
    ```
-   **Recommended IDE**:
    -   [JetBrains Rider](https://www.jetbrains.com/rider/) (with MAUI support)
    -   [Visual Studio 2022](https://visualstudio.microsoft.com/) (with the ".NET Multi-platform App UI development" workload installed)
-   **Flickr API Key**:
    -   A valid Flickr API key is required. Get yours [here](https://www.flickr.com/services/apps/create/apply/).

---

## 🛠️ Development

### Running Tests

This project uses [xUnit](https://xunit.net/) for unit testing.

To run the tests locally:

```bash
dotnet test
