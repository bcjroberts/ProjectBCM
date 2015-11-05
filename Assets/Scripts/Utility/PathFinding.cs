using UnityEngine;
using System.Collections.Generic;

public class PathFinding {

	public static List<Vector2> findPath(Vector2 startPos, Vector2 endPos, char[,] arrayData){

		List<Vector2> path = new List<Vector2> ();

		int[,] startArray = convertCharArrayToIntArray(arrayData);
		setArrayDistances (Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y), 0, startArray);

		/*string printinfo = "";
		for (int j = 0; j<startArray.GetLength(0); j++) {
			for(int k = 0;k<startArray.GetLength(1);k++){
				printinfo+= " "+startArray[j,k];
			}
			printinfo+="\n";
		}
		Debug.Log (printinfo);*/

		path.Add (endPos);
		int cx = Mathf.RoundToInt (endPos.x);
		int cy = Mathf.RoundToInt (endPos.y);
		int cvalue = startArray [cx, cy];
		bool done = false;
		int cIter = 0;
		int maxIter = startArray.GetLength (0) * startArray.GetLength (1);
		while (!done && cIter<maxIter) {

			if (cx - 1 >= 0 && startArray [cx - 1, cy]!=-1 && startArray [cx - 1, cy] < cvalue){
				cx -= 1;
				path.Add(new Vector2(cx, cy));
				cvalue = startArray[cx, cy];
			}else if (cx + 1 < startArray.GetLength(0) && startArray [cx + 1, cy]!=-1 && startArray [cx + 1, cy] < cvalue){
				cx += 1;
				path.Add(new Vector2(cx, cy));
				cvalue = startArray[cx, cy];
			}else if (cy - 1 >= 0 && startArray [cx, cy-1]!=-1 && startArray [cx, cy - 1] < cvalue){
				cy -= 1;
				path.Add(new Vector2(cx, cy));
				cvalue = startArray[cx, cy];
			}else if (cy + 1 < startArray.GetLength(1) && startArray [cx, cy+1]!=-1 && startArray [cx, cy + 1] < cvalue){
				cy += 1;
				path.Add(new Vector2(cx, cy));
				cvalue = startArray[cx, cy];
			}
			if(cvalue==0){
				done = true;
			}
			cIter++;
		}
		return path;
	}
	//looks at a char array and returns an int array for path finding
	public static int[,] convertCharArrayToIntArray(char[,] charArrayData){
	
		int[,] returnArray = new int[charArrayData.GetLength(0),charArrayData.GetLength(1)];
		
		for(int j = 0;j<returnArray.GetLength(0);j++){
			for(int k = 0;k<returnArray.GetLength(1);k++){
				
				//Beautiful!!!!!
				returnArray[j,k] = new List<char>{'s'}.Contains(charArrayData[j,k]) ? -1 : returnArray.GetLength(0)*returnArray.GetLength(1);
			}
		}
		return returnArray;
	}
	private static void setArrayDistances(int x, int y, int distance, int[,] array){

		if (array [x, y] > distance)
			array [x, y] = distance;

		if (x - 1 >= 0 && array [x - 1, y] > distance + 1)
			setArrayDistances (x - 1, y, distance + 1, array);

		if (x + 1 < array.GetLength(0) && array [x + 1, y] > distance + 1)
			setArrayDistances (x + 1, y, distance + 1, array);

		if (y - 1 >= 0 && array [x, y - 1] > distance + 1)
			setArrayDistances (x, y - 1, distance + 1, array);

		if (y + 1 < array.GetLength(1) && array [x, y + 1] > distance + 1)
			setArrayDistances (x, y + 1, distance + 1, array);

	}

}
