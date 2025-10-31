using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models;

public partial class Campus
{
    public int CampusId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

