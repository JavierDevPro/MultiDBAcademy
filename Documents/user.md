# User Module Documentation

The **User** module manages all information and operations related to users within the MultiDBAcademy system. It is designed following a layered architecture, which keeps the code organized, scalable, and easy to maintain.

---

## 1. Module Purpose

The User module allows:

- Retrieving all registered users.
- Getting information about a specific user.
- Updating user data.
- Deleting a user.

This module **does NOT handle registration or login**; those functions belong to the Auth module.  
The User module only manages users that already exist.

---

## 2. General Workflow

The internal workflow follows these steps:

1. The API receives the request (GET, PUT, DELETE).
2. The **controller** validates the data and forwards the action to the service.
3. The **UserService** applies business rules.
4. The **UserRepository** interacts with the database.
5. **AutoMapper** converts between entities and DTOs.
6. The API returns the processed data to the client.

### Components Involved

- **Controller:** processes HTTP requests.
- **Service:** contains business logic.
- **Repository:** manages database communication.
- **DTO:** controls what data is sent or received.
- **Entity:** represents the model stored in the database.

---

## 3. Module Features

### 3.1 List all users
Returns the complete list of users stored in the system.

### 3.2 Get a user by ID
Retrieves information for a specific user.  
If the ID does not exist, a proper message is returned.

### 3.3 Update a user
Allows updating permitted fields of an existing user:

- UserName
- Email
- RoleId
- Update date (handled automatically)

Sensitive or structural fields cannot be modified.

### 3.4 Delete a user
Deletes an existing user, after validating that the user actually exists.

---

## 4. Important Rules and Validations

- The module does **not** handle passwords or update PasswordHash.
- It does not modify the user's ID.
- Only explicitly allowed fields are updated.
- CreatedAt never changes.
- Deletion requires prior validation.
- The module does not handle tokens or authentication.

---

## 5. Security

To protect sensitive information:

- DTOs are used to expose only safe data.
- PasswordHash, RefreshToken, and internal fields are never exposed in responses.
- The service controls which fields can be updated.
- The repository is the only component allowed to access the database.

---

## 6. Relationship With Other Modules

- **Auth:** handles registration, login, tokens, and passwords.
- **Roles:** determines permissions and access levels.
- **Instances:** users may have assigned database instances, but their management belongs to another module.

---

## 7. Summary

The User module provides:

- Complete management of existing users.
- Strict update control with clear rules.
- Protection against exposing sensitive information.
- Well-organized, layered structure.
- AutoMapper for model transformation.
- A secure, maintainable, and extensible CRUD.

---
