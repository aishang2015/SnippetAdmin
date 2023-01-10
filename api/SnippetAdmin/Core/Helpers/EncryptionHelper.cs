using System.Security.Cryptography;
using System.Text;

namespace SnippetAdmin.Core.Helpers
{
	public static class EncryptionHelper
	{
		/// <summary>
		/// MD5
		/// </summary>
		public static string MD5Hash(string encryptStr)
		{
			using var md5 = MD5.Create();
			var sb = new StringBuilder();
			foreach (var hashByte in md5.ComputeHash(Encoding.UTF8.GetBytes(encryptStr)))
			{
				sb.Append(hashByte.ToString("X2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// SHA256
		/// </summary>
		public static string SHA256Hash(string encryptStr)
		{
			var sha256Generator = SHA256.Create();
			var sb = new StringBuilder();
			foreach (var hashByte in sha256Generator.ComputeHash(Encoding.UTF8.GetBytes(encryptStr)))
			{
				sb.Append(hashByte.ToString("X2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Aes加密
		/// </summary>
		/// <param name="encryptStr">需要加密的字符串</param>
		/// <param name="encryptKey">加密密钥，32非中文字符</param>
		/// <param name="iv">向量，16非中文字符</param>
		/// <returns>加密后的字符串</returns>
		public static string AesEncrypt(string encryptStr, string encryptKey, string iv = null)
		{
			using var aesInstance = Aes.Create();
			aesInstance.Key = Encoding.UTF8.GetBytes(encryptKey);
			aesInstance.IV = string.IsNullOrEmpty(iv) ? new byte[16] : Encoding.UTF8.GetBytes(iv);

			var encryptStrBytes = Encoding.UTF8.GetBytes(encryptStr);

			using var encryptor = aesInstance.CreateEncryptor();
			var encryptBytes = encryptor.TransformFinalBlock(encryptStrBytes, 0, encryptStrBytes.Length);
			return Convert.ToBase64String(encryptBytes);
		}

		/// <summary>
		/// Aes解密
		/// </summary>
		/// <param name="decryptStr">需要解密的字符串</param>
		/// <param name="decryptKey">加密密钥，32非中文字符</param>
		/// <param name="iv">向量，16非中文字符</param>
		/// <returns>解密后的字符串</returns>
		public static string AesDecrypt(string decryptStr, string decryptKey, string iv = null)
		{
			using var aesInstance = Aes.Create();
			aesInstance.Key = Encoding.UTF8.GetBytes(decryptKey);
			aesInstance.IV = string.IsNullOrEmpty(iv) ? new byte[16] : Encoding.UTF8.GetBytes(iv);

			var decryptStrBytes = Convert.FromBase64String(decryptStr);

			using var decryptor = aesInstance.CreateDecryptor();
			var encryptBytes = decryptor.TransformFinalBlock(decryptStrBytes, 0, decryptStrBytes.Length);
			return Encoding.UTF8.GetString(encryptBytes);
		}
	}
}