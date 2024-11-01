﻿namespace PhotoGallery.Models.RepositoryDTOs;

public sealed class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
