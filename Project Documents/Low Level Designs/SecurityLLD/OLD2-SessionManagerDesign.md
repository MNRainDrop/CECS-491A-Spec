# Auth and session manager redesign
## Classes involved
- Session Manager
- Auth Service
- DB Auth Target
- AuthUserModel
- hashedOTP
- Simplified DAO functionality (for straight SQL commands)

## Services Layer
### **Set OTP passcode**
- Generates and stores an OTP to the database
- Should be called upon first login attempt, so we can start with a new OTP to work with
```mermaid
sequenceDiagram
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

sm->>as: Response updateOTP(userAuthModel)
as->>at: storeOTP(userID)
at-->>as: Response Object<br/> with 
as--xls: Log.log()<br/>Logging OTP update call
at-->>as: Response Object<br/>with no errors
as-->>sm: Response Object<br/>with outcome
```

### **Get User model**
- Returns a model of the current user, based on their user name
```mermaid
sequenceDiagram
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

sm->>as: Response getUserAuthModel(userName)
as->>at: retrieveUserAuthModel(userID)
at->>dao: Response executeReadSQL(sql)
dao->>ds: cmd.ExecuteReader()
ds-->>dao: Returns Raw rows
dao-->>at: Response Object<br/> with outcome and rows
at-->>as: Response object<br/> with user model 
as--xls: Log.log()<br/>log user Model request
as-->>sm: Response Object<br/>with userAuthModel
```

### **Get user principal**
- Returns the current User principal, regardless of their login status
```mermaid
sequenceDiagram
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

sm->>as: Response getUserprincipal(userModel)
as->>at: retrieveUserClaims(userID)
at->>dao: Response executeReadSQL(sql)
dao->>ds: cmd.executeReader()
ds-->>dao: returns raw rows
dao-->>at: Response Object<br/>with outcome and rows
at-->>as: Response Object<br/> with User Principal
as--xls: Log.log()<br/> log user principal retrieval
as-->>sm: Response Object<br/>with userPrincipal
```

### **Get User OTP**
- Retrieves the hashed OTP from the database
```mermaid
sequenceDiagram
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

sm->>as: Response getOTPHash(userModel)
as->>at: retrieveOTPHash(userID)
at->>dao: Response executeReadSQL(sql)
dao->>ds: cmd.executeReader()
ds-->>dao: returns raw rows
dao-->>at: Response Object<br/>with outcome and rows
at-->>as: Respone Object<br/>with userOTP
as--xls: Log.log()<br/>log userOTP retrieval
as-->>sm: Response Object<br/>with hashedOTP
```

### **Authenticate**
- Takes in a user Authentication request, and compares it to the hashed value stored in the database
- This is ran on backend as a service, to ensure items like the pepper never end up at the front end
```mermaid
sequenceDiagram
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

sm->>as: Authenticate(AuthRequest, hashedOTP)
as->>as: hash(AuthRequest.Proof)
as->>as: compare hashes
as--xls: Log.log()<br/>log authentication request
as-->>sm: Response Object<br/>Bool containing outcome
```

## Manager Layer
### **Log In attempt**
- Note: this is not where we authenticate the user, this is simply what happens when we write down our username and click the "Login"
  - THis is done before we start recording log in attempts, but we will regenerate the otp the moment we click this button
- We start fresh and do the following in order:
  - Retrieves the user profile (Stores the user model to member value) 
  - Retrieves the User principal (does not store to attribute variable, simply holds it in method)
  - Checks the user principal to ensure that the user can log in
  - Goes through the full process of creating a OTP uploading it, then validating it

#### Login attempt Success case
```mermaid
sequenceDiagram
participant ep as Entry Point
participant sm as Session Manager
participant ls as Logging service
participant as as AuthService
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

ep->>sm: attemptLogin(username)
sm->>as: getUserAuthModel(username)
as->>as: (Refer to earlier section)
as-->>sm: Response Object<br/>with userAuthModel
sm->>as: getUserPrincipal(userAuthModel)
as->>as: (Refer to earlier section)
as-->>sm: Response Object<br/>with userPrincipal
sm->>sm: Validate Login claim<br/>We assume success
sm->>as: updateOTP(userAuthModel)
as->>as: (Refer to earlier section)
as-->>sm: Response Object<br/>confirming successful storage
sm--xls: Log.log()<br/> Record Login Attempt
```
#### Login Attempt Failure case: User does not exist
#### Login Attempt Failure case: Login claim is false

### **Authenticate User**
- Note: This must always be called after `Login Attempt` feature, or it will always fail
- Based on the locally stored user model, we are going to take a user provided OTP and  compare it to the databased stored value

#### Authenticate attempt success case: Correct OTP, successful authentication
```mermaid
sequenceDiagram
participant ep as Entry Point
participant sm as Session Manager
participant ls as Logging service
participant as as AuthService
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

ep->>sm: Authenticate(proof)
sm->>as: getUserPrincipal(userID)
as->>as: (Refer to earlier section)
as-->>sm: Response Object<br/>with user principal
sm->>sm: We create an Authentication Request
sm->>as: getOTPHash(userID)
as->>as: (Refer to earlier section)
as-->>sm: response with storedOTPhash
sm->>as: Authenticate(authRequest, storedOTPHash)
as->>as: (Refer to earlier section)
as-->>sm: Response object<br>with yes or no bool
sm->>sm: Stores the principal to object attribute
sm->>ls: Log.log()<br/>logs authentication success
```

#### Authenticate attempt failure case: No user model loaded
- Note: This case will happen if account does not exist or if they are not allowed to log in, due to Login Attempt step
#### Authenticate attempt failure case: wrong password given prior to last attempt
#### Authenticate attempt failure case: Wrong password given on last attempt
#### Authenticate attempt failure case: Max login attempts reached
- This should lock out their account for a certain period of time

### **Refresh User Principal**
- Gets an updated version of the user principal
```mermaid
sequenceDiagram
participant ep as Entry Point
participant sm as Session Manager
participant ls as Logging Service
participant as as AuthService
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

ep->>sm: refreshPrincipal()
sm->>sm: checks if a user principal exists<br/>(We are assuming a success case scenario)
sm->>as: getUserPrincipal(userID)
as->>as: (Refer to earlier section)
as-->>sm: Response Object<br/>with User Principal
sm->>sm: stores User Principal<br/>to member variable
sm--xls: Log.log()<br/>logs User Principal Refresh
```

### **Log out**
- Deletes the current principal from our system, and deletes the model and principal from our session manager object
```mermaid
sequenceDiagram
participant ep as Entry Point
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

ep->>sm: logOut()
sm->>sm: writes Null value to userModel Object
sm->>sm: writes Null value to userAccount Object
sm--xls: Log.log()<br/>Log logout attempt
sm-->>ep: Returns Response with Logout attempt
```

### **Authorize**
- This is where we check if a claim is present on our userPrincipal
- We do this by passing in required claims, and validating them against the current user principal

```mermaid
sequenceDiagram
participant ep as Entry Point
participant sm as Session Manager
participant as as AuthService
participant ls as Logging Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

ep->>sm: authorize(requiredClaim, requiredScope)
sm->>sm: Checks if principal is expired
alt User Principal expired
  sm->>sm: bool refreshPrincipal()<br/>Check refresh diagram
end
sm->>sm: checks if required claim is<br/>present in the principal
sm--xls: Log.log()<br/>Log authz request
sm-->>ep: returns Boolean value<br/>stating whether or not they are authorized
```