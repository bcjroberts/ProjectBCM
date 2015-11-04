using UnityEngine;
using System.Collections.Generic;

public class PathFinding {

	public static void findPath(Vector2 startPos, Vector2 endPos, char[,] arrayData){
		
		int[,] startArray = convertCharArrayToIntArray(arrayData);
		
		
		
	}
	//looks at a char array and returns an int array for path finding
	public static int[,] convertCharArrayToIntArray(char[,] charArrayData){
	
		int[,] returnArray = new int[charArrayData.GetLength(0),charArrayData.GetLength(1)];
		
		for(int j = 0;j<returnArray.GetLength(0);j++){
			for(int k = 0;k<returnArray.GetLength(1);k++){
				
				//Beautiful!!!!!
				returnArray[j,k] = new List<char>{'s'}.Contains(charArrayData[j,k]) ? -1 : 0;
			}
		}
		return returnArray;
	}
}
