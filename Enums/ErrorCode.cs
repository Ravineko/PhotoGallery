namespace PhotoGallery.Enums;

public enum ErrorCode
{
    UserNotFound = 1,
    WrongPassword,
    EmailNotSent,
    UserNameAlreadyExist,
    UserEmailAlreadyExist,
    TwoFactorCodeWasntFound,
    TwoFactorCodeWasUsed,
    TwoFactorCodeNotMatch,
    RefreshTokenWasntFound,
    RefreshTokenExpired,
    InternalServerError,
    None,
    InvalidAccessToken,
    PromotionCodeNotFound,
    PromotionCodeNotMatch
}
