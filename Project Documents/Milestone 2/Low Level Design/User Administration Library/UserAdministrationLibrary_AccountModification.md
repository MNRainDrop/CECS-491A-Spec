:::mermaid

sequenceDiagram




    # See BRD UA-6 Account Modification

    participant ml as Manager Layer
    participant ls as LoggingService<br>(Services Layer)
    participant as as AuthService<br>(Services Layer)
    participant uc as UserService<br>(Services Layer)
    participant dg as SqlDbUserTarget<br>(Data Gateway)
    participant da as SQLServerDAO
    participant ds as Data Store

%% Depending on AuthZ implementation, We may want to check if the user is auhtorized over a time duration as well 

    ml->>+as: 'AppAuthService isAuthorized(AppPrincipal currentPrincipal, IDictionary<string> <string> requiredClaims)'
        Note over ml,as: Manager Layer calls upon<br>  'AppAuthService isAuthorized(AppPrincipal currentPrincipal, IDictionary<string> <string> requiredClaims)'<br>to see if user has correct permissions
        Note right of as: See Low Level Design for Authorization

    ml->>+uc: IAccountResponse modifyUser(IUserModel)
        Note over ml,uc: Manager Layer calls upon<br>`IAccountResponse modifyUser(IUserModel)`<br>to modify the user

    # UserService should check/ validate data passed through
    uc->>uc: UserService checks to see if the `IResponse` object has errors or not

    # Modify the user
     uc->>+dg: IAccountResponse generateAccountModificationSQL(IUserModel)
     Note over uc,dg: UserService calls upon<br>`IAccountResponse generateAccountModifcationSQL(IUserModel)`<br>to pass user data to be written to a parameterized SqlCommand




    dg->>dg: SqlDbUserTarget creates Parameterized SqlCommand




    dg->>+da: IResponse executeWriteSQL(SqlCommand sql)
    Note over dg,da: SqlServerUserTarget calls upon<br>`IResponse executeWriteSQL(SqlCommand sql)`<br>to write the sql




    da->>+ds: SqlCommand.ExecuteNonQuery()
    Note over da,ds: SqlServerDAO calls upon<br>`SqlCommand.ExecuteNonQuery()`<br>to execute the sql command




    ds->>ds: User account data is successfully<br>modified in the Data Store




    ds-->>-da: Database returns the number of rows affected




    da-->>-dg: IResponse object returned with<br>`Response.ReturnValue` equal to the retrieved value




    dg-->>-uc: IAccountResponse object returned with<br>`IAccountResponse.HasError` equal to false




    uc-->>-ml: IAccountResponse object returned


    ml ->> ls: 'IResponse createLog(string logLevel, string logCategory, string logContext, string? createdBy= null)'
         Note over ml,ls: LoggingService calls upon<br>'IResponse createLog(string logLevel, string logCategory, string logContext, string? createdBy= null)'<br> to log the modification was successful
        Note right of ls: See Low Level Design for Logging
:::