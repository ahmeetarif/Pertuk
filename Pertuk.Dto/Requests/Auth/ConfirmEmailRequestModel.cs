﻿namespace Pertuk.Dto.Requests.Auth
{
    public class ConfirmEmailRequestModel
    {
        public string UserId { get; set; }
        public string DigitCode { get; set; }
    }
}