# 🎓 Online Examination System – ASP.NET Core

A scalable **Online Examination System** built using **ASP.NET Core Web API**, **Entity Framework Core**, and **JWT Authentication**, following **Clean Architecture**, **Onion Architecture**, and **Vertical Slicing** principles.

This project demonstrates real-world backend development practices including role-based authorization, complex relationships, and exam lifecycle management.

---

## 🚀 Key Features

### 🔐 Authentication & Authorization
- JWT-based authentication
- Login via **Email or Username**
- Role-based access control (**Student / Instructor**)

### 👨‍🎓 Student Capabilities
- Enroll in courses
- View assigned exams
- Start timed exams
- Submit answers
- Automatic exam submission when time expires
- View exam results and scores

### 👨‍🏫 Instructor Capabilities
- Create and manage courses
- Create exams and questions
- Assign exams to courses
- Assign students to courses
- View student answers and results

### 🧠 Exam Management
- Multiple-choice questions
- Timed exams
- Automatic grading
- Final exam restriction (one attempt per student)

---

## 🏗 Architecture & Design

- N-Teir Architecture
- Repository Pattern
- Service Layer abstraction
- AutoMapper for DTO ↔ ViewModel mapping
- Unified API response structure

---

## 🛠 Tech Stack

- ASP.NET Core Web API
- Entity Framework Core (Code First)
- SQL Server
- JWT Authentication
- AutoMapper
- Swagger (OpenAPI)
- LINQ & Asynchronous Programming

---

## 🗄 Database Design

- Table-Per-Type (TPT) inheritance
- `User` base entity
- `Student` and `Instructor` derived entities
- Soft delete strategy
- Many-to-many relationships:
  - Student ↔ Course
  - Student ↔ Exam

---

## 📄 API Response Format

All endpoints return a standardized response:

```json
{
  "isSuccess": true,
  "data": {},
  "message": null,
  "errorCode": null
}
