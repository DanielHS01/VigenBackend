using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Vigen_Repository.Email
{
    public class Reset
    {
        public string EnviarNuevaContrasena(string correo, string nuevaContrasena)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("vigenservice@gmail.com"));
                email.To.Add(MailboxAddress.Parse(correo));
                email.Subject = "Restablecimiento de contraseña - Vigen Security Service";

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto;'>
                            <h2 style='color: #1a73e8;'>Vigen Security Service</h2>
                            <p>Hola {correo},</p>
                            <p>Hemos generado una nueva contraseña para tu cuenta:</p>
                            <div style='font-size: 24px; font-weight: bold; color: #333; background-color: #e9ecef; padding: 10px; border-radius: 5px; text-align: center;'>
                                {nuevaContrasena}
                            </div>
                            <p>Te recomendamos iniciar sesión y cambiarla lo antes posible por una que recuerdes fácilmente.</p>
                            <p style='font-size: 12px; color: #777;'>Si no solicitaste este cambio, por favor contáctanos inmediatamente.</p>
                            <hr style='border: none; border-top: 1px solid #ddd; margin-top: 20px;' />
                            <p style='font-size: 12px; color: #777;'>Este mensaje fue enviado automáticamente por Vigen Security Service.</p>
                        </div>
                    </body>
                </html>"
                };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("vigenservice@gmail.com", "hagzssosuisaozhy"); // Contraseña de aplicación
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return ex.Message;
            }
        }
    }
}
