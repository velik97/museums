using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class LeaderboardToXlsx
{		
	public static string SaveXlsx(List<Score> scores, string folderName, string fileName)
	{
		scores.Sort();
		
		Dictionary<int, List<Score>> museumScoresLists = new Dictionary<int, List<Score>>();

		for (int i = 0; i < 3; i++)
		{
			List<Score> museumScores = scores.FindAll(o => o.locationId == i);
			if (museumScores.Count > 0)
			{
				museumScoresLists.Add(i, museumScores);
			}
		}

		Excel xlsx = new Excel();
		int tablesCount = 0;

		foreach (var key in museumScoresLists.Keys)
		{
			string tableName = GameInfo.LocationByIndex(key);
			if (tableName == "")
				tableName = "???";
			
			xlsx.AddTable(tableName);
			ExcelTable table = xlsx.Tables[tablesCount];
			tablesCount++;

			table.SetValue(1, 1, "№");
			table.SetValue(1, 2, "Имя");
			table.SetValue(1, 3, "Статус");
			table.SetValue(1, 4, "очки");

			List<Score> museumScores = museumScoresLists[key];
			
			for (var i = 0; i < museumScores.Count; i++)
			{
				int row = i + 2;
				table.SetValue(row, 1, (i + 1).ToString());
				table.SetValue(row, 2, museumScores[i].name);
				table.SetValue(row, 3, museumScores[i].status);
				table.SetValue(row, 4, museumScores[i].points.ToString());
			}						
		}

		string path = folderName + "\\" + fileName + ".xlsx";

		int fileNumber = 0;

		while (File.Exists(path))
		{
			fileNumber++;
			path = folderName + "\\" + fileName + " " + fileNumber.ToString() + ".xlsx";
		}
		
		ExcelHelper.SaveExcel(xlsx, path);

		return fileNumber == 0 ? fileName : (fileName + " " + fileNumber.ToString());
	} 
}
