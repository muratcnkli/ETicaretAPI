using ETicaretAPI.Insfrastructure.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Insfrastructure.Services.Storage
{
	public class Storage
	{
		protected delegate bool HasFile(string pathOrContainerName, string fileName);
		protected async Task<string> FileRenameAsync(string pathOrContainerName, string fileName,HasFile hasFileMethod, bool first = true)
		{
			string newFileName = await Task.Run<string>(async () =>
			{
				string extension = Path.GetExtension(fileName); // Dosyanın uzantısını alıyoruz.
				string newFileName = string.Empty;
				if (!true)
				{
					string oldName = Path.GetFileNameWithoutExtension(fileName); //Dosyanın ismini alıyoruz.
					newFileName = $"{NameOperation.CharacterRegulatory(fileName)}{extension}"; // Dosyanın yeni ismini ve uzantısını birleştirip düzenliyoruz.
				}
				else
				{
					newFileName = fileName;
					int indexNo1 = newFileName.IndexOf('-');
					if (indexNo1 == -1)
					{
						newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
					}
					else
					{
						int lastIndex = 0;
						while (true)
						{
							lastIndex = indexNo1;
							indexNo1 = newFileName.IndexOf("-", indexNo1 + 1);
							if (indexNo1 == -1)
							{
								indexNo1 = lastIndex;
								break;
							}
						}
						int indexNo2 = newFileName.IndexOf(".");
						string fileNo = newFileName.Substring(indexNo1 + 1, indexNo2 - indexNo1 - 1);
						if (int.TryParse(fileNo, out int _fileNo))
						{
							_fileNo++;
							newFileName = newFileName.Remove(indexNo1 + 1, indexNo2 - indexNo1 - 1)
																		.Insert(indexNo1 + 1, _fileNo.ToString());
						}
						else
						{
							newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";

						}

					}
				}

				if (hasFileMethod(pathOrContainerName,newFileName))//Aynı path üzerinde aynı dosya var ise
				{
					return await FileRenameAsync(pathOrContainerName, newFileName,hasFileMethod,false);
				}
				else
				{
					return newFileName;
				}

			});
			return newFileName;
		}

	}
}
