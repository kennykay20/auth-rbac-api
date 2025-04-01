namespace crud_api.Dtos;

public class ApiResponse<T>
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public int Status { get; set; }
  public T? Data { get; set; }
}

public class ApiListResponse<T>
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public int Status { get; set; }
  public List<T>? Data { get; set; }
}

public class ApiListPageResponse<T>
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public int Status { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }
  public int Count { get; set; }
  public List<T>? Data { get; set; }
}

public class ApiTokenResponse
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public int Status { get; set; }
  public string AccessToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
  public Guid Uuid { get; set; }
  public required string RefreshToken { get; set; }
}
public class ApiResponseUser
{
  public int Id { get; set; }
  public Guid Uuid { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; }
  public string? Password { get; set; }
  public string Roles { get; set; } = string.Empty;
  public bool IsActive { get; set; }
  public bool IsNewUser { get; set; }
  public string Otp { get; set; } = string.Empty;
  public DateTime? OtpExpiry { get; set; }
  public string RegistrationToken { get; set; } = string.Empty;
  public bool IsDeleted { get; set; }
}