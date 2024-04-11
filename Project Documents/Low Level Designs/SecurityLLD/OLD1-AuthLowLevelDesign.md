# Low Level Design Milestone 2
## Token Manager
### Main functionalities
- The goal is to create a manager that can create tokens, hold tokens, and use the tokens in order to authorize all actions. 

### Deliverables
#### Classes
- SessionManager
```mermaid
classDiagram
class SessionManager{
  - userPrincipal currentPrincipal
  + Response AuthenticateUser(username, password)
  + bool Authorize()
  + Response refreshPrincipal()
  + bool LogOut() nulls the currentPrincipal
}
```
## Security Library For Authentication
### Main Functionalities
### Deliverables
```mermaid
erDiagram
userAccount|o--o{ Logs: creates
userAccount ||--o| Pass: AuthenticatesWith
userAccount ||--o{ userClaims: can
claims ||--o{ userClaims: populates
Pass{
  UID bigInt
  passHash string
}
userAccount{
    userID bigInt
    userName string
    salt UInt32
    userHash string
}
claims{
  claimID int
  claim string
}
userClaims{
  userHash string
  claimID int
  claimScope bool
}
Logs{
  logID bigInt
  logTime dateTIme
  userHash string
  logLevel string
  logCategory string
  logContext string
  logHash string
}
```

#### Current models
```mermaid
classDiagram
class userAccount{
  userID bigInt
  userName string
  salt int
  userHash string
}
```

### Implementation
#### Retrieve account
```mermaid
sequenceDiagram
participant ep as Entry Point
participant as as AuthService
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

Note left of ep: Something has requested<br>userAccount information
ep->>+as: Response getUserAccountInfo(string username)
as->>+at: Response getUserAccount(string username)
at->>+dao: Response executeReadSQL(string sql)
dao->>+ds: cmd.ExecuteReader()
ds->>ds: obtains account tied to the user name requested
ds-->>-dao: Returns rows
dao-->>-at: Returns Response object with row Values
at-->>-as: Returns Response object<br>with UserAccount Object
as-->>-ep: Returns Response object with success results<br>Response contains UserAccount Object
```
#### Hashing
- `string hashPass(int salt, int pepper, string pass)`
```mermaid
sequenceDiagram
participant ep as Entry Point
participant hs as Hash Service

Note left of ep: Something has called<br>to hash a password

ep->>+hs: string hashPass(int salt, int pepper, string pass)
hs->>hs: byte[] saltAndPepper = BitConverter.GetBytes(salt).Concat(BitConverter.GetBytes(pepper)).ToArray
Note over hs: This combines the salt and pepper into one "Salt"
hs->>hs: byte[] Pbkdf2 (string pass, byte[] salt = saltAndPepper,<br>int iterations = 10000, hashAlgorithm = sha256, int outputLength = 256)
Note over hs: This hashes the pass into a byte array
hs->>hs: BitConverter.ToString(hash).Replace("-", "")<br>convert byte[] to HexadecimalString.
Note over hs: Converts the byte array into hex string
hs-->>-ep: return hexidecimal string
``` 
- `string hashUser(string username, int pepper)`
```mermaid
sequenceDiagram
participant ep as Entry Point
participant hs as Hash Service

Note left of ep: Something has called<br>to hash a username

ep->>+hs: string hashUser(string username, int pepper)
hs->>hs: byte[] userBytes = Encoding.UTF8.GetBytes(username)<br>byte[] pepperBytes = BitConverter.GetBytes(pepper)<br>byte[] saltedUsername = userBytes.Concat(pepperBytes).ToArray()
Note over hs: creates a combined salted user byte array
hs->>hs: byte[] hash = sha256.ComputeHash(saltedUsername)
Note over hs: creates the userHash as a byte array
hs->>hs: string hashString = BitConverter.ToString(hash).Replace("-", string.Empty)
Note over hs: converts the byte array to hex string
hs-->>ep: returns hashed string value
```


#### Hashing and storing a passcode
```mermaid
sequenceDiagram
participant ep as Entry Point
participant as as AuthService
participant ps as Pepper Service
participant hs as Hashing Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

Note left of ep: Something has called<br>to store a password

ep->>+as: Response storePass(UserAccount userIdentity, string pass)
as->>+ps: int getPepper()
ps->>-as: Returns pepper int
as->>+hs: string hashPass(int salt, int pepper, string pass)
hs->>-as: returns hexidecimal passHash
as->>+at: Response saveHashedPass(long UID, string passHash)
at->>+dao: Response executeWriteSQL(string sql)
dao->>+ds: cmd.ExecuteNonQuery()
ds->>ds: updates rows in datastore
ds-->>-dao: returns rows modified
dao-->>-at: Returns response object with success case
at-->>-as: Returns response object with success case
as-->>-ep: Returns response object with success case
```
##### Retrieving User Principal
```mermaid
sequenceDiagram
participant ep as Entry Point
participant as as AuthService
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

ep->>+as: Response getPrincipal(UserAccount userIdentity)
as->>+at: Response getClaims(long UID)
at->>+dao: Response executeReadOnly(string sql)
dao->>+ds: cmd.ExecuteReader()
ds->>ds: gets all claims for user
ds-->>-dao: returns rows
dao-->>-at: Response object with sql rows
at-->>-as: Response object with claims Dict
as-->>-ep: Response object with AppPrincipal
```

#### Validating a pass
```mermaid
sequenceDiagram
participant ep as Entry Point
participant as as AuthService
participant ps as Pepper Service
participant hs as Hash Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

Note left of ep: Something has called<br>to validate a password
ep->>+as: Response validatePass(AuthenticationRequest authRequest)
as->>+at: Response fetchPass(int UID)
at->>+dao: Response executeReadSQL(string sql)
dao->>+ds: cmd.ExecuteReader()
ds->>ds: obtains hashedPass tied to the user ID requested
ds-->>-dao: Returns row
dao-->>-at: Response object with sql row item
at-->>-as: Response object with string hashedPass
as->>+ps: int getPepper()
ps-->>-as: integer with pepper value returned
as->>+hs: string hashPass(UserAccount userIdentity, string pass)<br>Hashing the user's proof
hs-->>-as: return hexidecimal string
as-->>as: compare hashedProof to hashedPass
as-->>-ep: return Response with bool based on acceptance
```

#### Authenticating a user
```mermaid
sequenceDiagram
participant ep as Entry Point
participant as as AuthService
participant hs as Hash Service
participant at as AuthTarget
participant dao as DAO Object
participant ds as Data Store

Note left of ep: we are assuming something<br>wants to authenticate a user

ep->>+as: AuthenticationResponse Authenticate(AuthenticationRequest authRequest)
as->>as: validate User Inputs
as->>as: Response getPrincipal(UserAccount userIdentity)<br>Response contains principal
as->>as: bool isAuthorize(AppPrincipal appPrincipal, Dict<string,string> requiredClaims)<br>checks if the user can login
Note over as: We are assuming True is returned
as->>as: Response validatePass(AuthenticationRequest authRequest)
Note over as: We assume Response with True value is returned
as-->>-ep: We return AuthenticationResponse with success case and good AppPrincipal
```
