:::mermaid

%% Need to update diagram to reflect actual account recovery
%% 1. User Deletes Account —> this will still leave a User’s ID hash
%% —-> User ID hash will still have information relating to that user…
%% 2A) User is checked if authorized to access service
%% 2B)  User Initiates a account recovery request
%% 		Information should be included: Users email
%% 3. Manager accesses UserRecoveryService
%% 4. Service will check if User ID hash exists
%% 5. If found, Account will become active again


sequenceDiagram




%% See BRD UA-2 for additional context




participant ml as Manager Layer
participant uc as UserService<br>(Services Layer)
participant dg as SqlDbUserTarget<br>(Data Gateway)
participant da as SQLServerDAO
participant ds as Data Store


ml->>+uc: IForgot isAuthorized(Forgot)
    Note over ml,uc: Manager Layer calls upon<br> 'IForgot isAuthorized(Forgot)'<br>to see if user has correct permissions


ml->>+uc: IAccountResponse recoverUser(IUserModel)
    Note over ml,uc: Manager Layer calls upon<br>`IAccountResponse recoverUser(IUserModel)`<br>to recover the user




%% UserService should check/ validate data passed through
uc->>+uc: UserService checks to see if the `IResponse` object has errors or not




alt IResponse.HasError is true
    uc-->>-ml: IAccountResponse object with errors returned
else




    %% Modify the user
    uc->>+dg: IAccountResponse generateAccountRcoverySQL(IUserModel)
    Note over uc,dg: UserService calls upon<br>`IAccountResponse generateAccountRecoverySQL(IUserModel)`<br>to pass user data to be written to a parameterized SqlCommand




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
end

:::