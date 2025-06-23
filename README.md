EventEase: Online Venue Booking System
Project Description
EventEase is an innovative event management company that requires an efficient, user-friendly online booking platform to streamline its operations. This application aims to simplify venue management, prevent booking conflicts, provide a clear view of scheduled events, and enable booking specialists to manage bookings effectively.

This platform is developed as an administrative tool for booking specialists. Customers can request bookings via external channels (email, phone, walk-in), and specialists use this system to facilitate and manage those bookings. The system accounts for scenarios where events may be planned and loaded before a venue becomes available for booking.

Features
The EventEase application provides the following key functionalities:

Comprehensive Venue Management (CRUD):

Create, Read, Update, Delete (CRUD) operations for venue information (name, location, capacity, image URL, availability status).

Venue Availability Tracking: A boolean field (IsAvailable) on venues to denote their current booking status.

Deletion Restriction: Prevents deletion of venues if they have active bookings associated with them, ensuring data integrity.

Image Uploads: Integrates with Azure Blob Storage for secure and scalable storage of venue images.

Robust Event Management (CRUD):

Create, Read, Update, Delete (CRUD) operations for event information (name, description, associated venue, and event type).

Event Type Classification: Events are categorized using predefined event types (e.g., Concert, Conference) for better organization and filtering.

Deletion Restriction: Prevents deletion of events if they have active bookings associated with them.

Streamlined Booking Management (CRUD with Advanced Features):

Create, Read, Update, Delete (CRUD) operations for customer bookings.

Double-Booking Prevention: Implements server-side validation to prevent overlapping bookings for the same venue on conflicting dates and times.

Basic Search: Allows searching bookings by BookingID or Event Name.

Advanced Filtering:

Filter bookings by Event Type.

Filter bookings by a custom Date Range.

Filter bookings based on the associated Venue's Availability.

Intuitive User Interface:

Built with Bootstrap for a responsive and consistent design across various devices (desktop, tablet, mobile).

Provides clear success and error messages using TempData for enhanced user experience.

Technologies Used
The EventEase application is built using the following technologies and Azure services:

Backend:

ASP.NET Core MVC: A cross-platform, high-performance, open-source framework for building modern, cloud-based, Internet-connected applications.

C#: The primary programming language used for all backend logic and application development.

Entity Framework Core (EF Core): An Object-Relational Mapper (ORM) that simplifies data access and management with the Azure SQL Database through LINQ queries and migrations.

Frontend:

HTML5: For structuring the web content.

CSS3: For styling and visual presentation.

Bootstrap: A popular CSS framework for responsive and mobile-first frontend development.

JavaScript: For client-side interactivity and dynamic elements.

Cloud Services (Microsoft Azure):

Azure App Service: Platform-as-a-Service (PaaS) for hosting the web application, providing easy deployment, scalability, and managed infrastructure.

Azure SQL Database: A fully managed relational database service in the cloud for storing all application data (venues, events, bookings, event types).

Azure Storage (Blob Storage): Used for scalable and cost-effective storage and management of venue images.

Setup and Installation (Local Development)
To run this project locally, follow these steps:

Prerequisites:

.NET SDK (latest stable version recommended, compatible with ASP.NET Core).

Visual Studio (Community Edition or higher).

Local SQL Server (for development database) or access to Azure SQL Database.

Git (for cloning the repository).

Clone the Repository:

git clone https://github.com/VCDN-2025/cldv6211-poe-ST10329226.git
cd cldv6211-poe-ST10329226

Configure Database:

Open the solution (EventEase.sln) in Visual Studio.

Update the DefaultConnection string in appsettings.json (and appsettings.Development.json) to point to your local SQL Server instance or your Azure SQL Database.

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventEaseDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  // Or your Azure SQL connection string
}

Open Package Manager Console in Visual Studio.

Run database migrations to create/update the database schema:

Update-Database

(Ensure you have at least one Venue record in your database before running this, as the migration assigns a default VenueID to existing Events).

Configure Azure Blob Storage (if implementing image uploads locally):

Add your Azure Storage connection string to appsettings.json.

Uncomment the Blob Storage code in VenuesController.cs if you intend to use it locally.

Run the Application:

In Visual Studio, press F5 or click Debug > Start Debugging.

The application should open in your default browser.

Deployment to Azure
This application is designed for deployment to Microsoft Azure:

Azure App Service: The web application is deployed as an Azure App Service.

Azure SQL Database: The database is hosted on Azure SQL Database.

Azure Blob Storage: Venue images are stored in Azure Blob Storage.

Deployment can be performed directly from Visual Studio using its built-in Publish wizard, selecting "Azure App Service (Windows)" as the target and ensuring the chosen App Service Plan SKU (e.g., Basic, Shared) complies with any Azure Policies in the subscription.

Contributing
Contributions are not expected for this academic project. However, in a real-world scenario, you would typically fork the repository, create a feature branch, commit your changes, and submit a pull request.

License
This project is for academic purposes and is not openly licensed for commercial use. For specific licensing details, please refer to any internal project documentation or contact the project owner.

Note: This README provides a general overview. For detailed instructions on specific features or advanced configurations, please refer to the project's internal documentation and code comments.
