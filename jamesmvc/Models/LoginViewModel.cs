﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jamesmvc.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
