using System.ComponentModel.DataAnnotations;

namespace crud_api.Models;

public class Permission {

  [Key]
  public int Id { get; set; }
  public string? Name { get; set; }
}