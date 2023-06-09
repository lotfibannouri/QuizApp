﻿using Authentication.web.Model;

namespace Authentication.web.Services
{
    public interface ICompteService
    {
        Task<Response> SignUpAsync(SignUpModel model);
        Task<Response> LoginAsync(LoginModel model);
        Task logout();
    }
}
