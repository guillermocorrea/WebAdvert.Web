# WebAdvert.Web

Sample Web client app, uses AWS Cognito user authentication.

## Config

Setup your User Pool at AWS Cognito.

Add your AWS Cognito configuration at `src/WebAdvert.Web/appsettings.json`:

```json
"AWS": {
    "Region": "",
    "UserPoolId": "",
    "UserPoolClientId": "",
    "UserPoolClientSecret": ""
  }
```

## Run

```sh
dotnet run -p src/WebAdvert.Web/
```
