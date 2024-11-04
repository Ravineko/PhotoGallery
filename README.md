### Readme for a project

##User Secrets structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_connection_string"
  },
  "JWTSettings": {
    "SecretKey": "YOUR_SECRET_KEY",
    "JWTExpireMinutes": "YOUR_JWT_EXPIRY_MINUTES"
  },
  "CodeSettings": {
    "TwoFAExpiryMinutes": "YOUR_2FA_EXPIRY_Minutes",
    "PromotionCodeExpiryMinutes": "YOUR_PROMOTIONCODE_EXPIRY_Minutes"
  },
  "EmailSettings": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "SendGridPromotionTemplateId": "YOUR_SENDGRID_TEMPLATE_ID",
    "SendGridResetTemplateId": "YOUR_SENDGRID_TEMPLATE_ID",
    "SendGridNewsletterTemplateId": "YOUR_SENDGRID_TEMPLATE_ID",
    "FromEmailAddress": "YOUR_FROM_EMAIL_ADDRESS",
    "FromEmailName": "YOUR_FROM_EMAIL_NAME",
    "Domain": "YOUR_DOMAIN"
  },
  "ImageSettings": {
    "ImageStoragePath": "wwwroot/images"
  }
}
```
