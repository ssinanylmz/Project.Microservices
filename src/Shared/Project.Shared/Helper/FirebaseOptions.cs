using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Shared.Helper
{
    /// <summary>
    /// Firebase yapılandırma seçeneklerini temsil eder.
    /// </summary>
    public class FirebaseOptions
    {
        /// <summary>
        /// Firebase proje kimliğini alır veya ayarlar.
        /// </summary>
        /// <value>
        /// Firebase proje kimliği.
        /// </value>
        public string? ProjectId { get; set; }

        /// <summary>
        /// Firebase API anahtarını alır veya ayarlar.
        /// </summary>
        /// <value>
        /// Firebase için API anahtarı.
        /// </value>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Firebase kimlik doğrulama alanını alır veya ayarlar.
        /// </summary>
        /// <value>
        /// Firebase kimlik doğrulama alanı.
        /// </value>
        public string? AuthDomain { get; set; }

        /// <summary>
        /// Firebase kimlik doğrulama için kimlik bilgilerinin dosya yolunu alır veya ayarlar.
        /// </summary>
        /// <value>
        /// Kimlik bilgilerinin saklandığı dosyanın yolu.
        /// </value>
        public string? CredentialPath { get; set; }
    }
}
