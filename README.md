1. Project Overview

This project is my final POE for PROG6212.
The Contract Monthly Claim System is a web app where:

Lecturers submit their monthly claims with supporting documents.

Programme Coordinators can view and verify claims.

Managers can review and approve/reject claims.

HR can manage users and generate reports for payroll.

The system is built with ASP.NET Core MVC, C#, Entity Framework Core and SQL Server.

2. Technologies Used

C# / ASP.NET Core MVC

Entity Framework Core + SQL Server

MSTest (for unit tests)

iTextSharp (for PDF report)

Bootstrap (basic styling)

3. How to Run the Project

Open the solution in Visual Studio.

Make sure SQL Server is installed and running.

Check appsettings.json → ConnectionStrings:DefaultConnection points to your SQL Server instance.

Build the solution.

Run the app (IIS Express or Kestrel) and log in using one of the seeded users or an HR-created user.

4. YouTube

([https://youtu.be/YOUR_VIDEO_ID_HERE](https://youtu.be/6pS1xvpM-EA))

5. Main Features (Quick Summary)

Lecturer

Login & dashboard

Submit Monthly Claim with:

Auto-filled name, email and hourly rate

Hours worked + notes

Multiple file uploads

View My Claims with status badges and downloadable documents

Coordinator / Manager

View all claims

Filter by status and date

See hours, totals, and documents

Download uploaded files

HR

Dashboard with totals (users, claims, approvals)

Manage users (create, edit, approve lecturers)

View all claims

Generate:

PDF report of approved claims

CSV export of lecturers

6. Unit Tests

A separate MSTest project is included:

Project: ContractMonthlyClaimSystem.Tests

It contains tests for:

Claim model:

Checks Total = HoursWorked × HourlyRate

Default status is Pending

CreatedUtc is set correctly

FileGuard service:

Accepts valid small PDFs

Rejects empty files

Rejects bad extensions

Rejects files over 5 MB

All tests pass and are visible in the Visual Studio Test Explorer.
