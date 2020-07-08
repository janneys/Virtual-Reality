using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLetter : MonoBehaviour
{
    public enum Wall { Left, Right, Back, Front };

    public GameObject letterPrefab;

    //Room
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject backWall;
    public GameObject frontWall;
    public GameObject ceilling;
    public GameObject ground;
    public GameObject door;

    //Margin
    public float leftMargin = 5.5f;
    public float topMargin = 5.5f;
    public float letterVerti = 6f;
    public float letterHori = 6f;
    public float letterCircle = 6f;

    //Number of letters
    public int letterColumns = 6;
    public int letterRows = 6;

    //Color
    public Color normalLetter = Color.black;
    public Color targetLetter = Color.red;

    //Task
    public int task1Repetitions = 5;
    public int task2Repetitions = 10;

    //The percentage the letter appears
    public float task2LetterAppear = 50;

    private List<GameObject> letters = new List<GameObject>();
    private GameObject letterDoor;

    private char[] letters_set1 = { 'A', 'K', 'M', 'N', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] letters_set2 = { 'E', 'F', 'H', 'I', 'L', 'T' };

    private int letterCount = 0;

    private bool isLettersCreated = false;
    private bool isLetterShownAtDoor = false;
    private bool isCamouflaged = true;
    private bool isTaskMode = false;
    private bool isAppearing = true;

    private int currentTask = 0;
    private int taskCount = 0;

    private char[] selectedSet;

    private char letter;

    private bool[] appear;

    private float lastTime = 0;
    private float deltaTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        selectedSet = letters_set1;

        /// ---- Log ----
        Debug.Log("Press 'T' to start Task Mode");
        Debug.Log("Press 'Space' to do task example");
        Debug.Log("Press 'C' to camouflage letter");
        Debug.Log("Press 'S' to change the current set of letters");
        Debug.Log("Press 'A' to change letter to appear");
        /// ---- Log ----
    }

    // Update is called once per frame
    void Update()
    {
        changeTaskModeInput();

        changeCamouflagedInput();

        changeSetInput();

        changeAppearingInput();

        startTaskInput();

    }

    // Select Task Mode 'T'
    public void changeTaskModeInput()
    {
        // If T key pressed, activate or desactivate Task Mode!
        // Task Mode is where the experiment starts 
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Remove all reset all
            removeLetterAtDoor();
            removeAllLetters();

            isTaskMode = !isTaskMode;

            if (!isTaskMode)
            {
                // The task is disabled so
                // There is not task running
                currentTask = 0;

                // The letters are appearing
                isAppearing = true;

                /// ---- Log ----
                Debug.Log("Task Disabled");

                /// ---- Log ----
                Debug.Log("Press 'T' to start Task Mode");
                Debug.Log("Press 'Space' to do task example");
                Debug.Log("Press 'C' to camouflage letter");
                Debug.Log("Press 'S' to change the current set of letters");
                Debug.Log("Press 'A' to change letter to appear");
                /// ---- Log ----
            }
            else
            {
                /// ---- Log ----
                Debug.Log("Task Enabled");
                Debug.Log("Press 1 or 2 to Choose Task");
                Debug.Log("Task 1: Pilot experiment with non-camouflaged letter, repeated " + task1Repetitions + " times");
                Debug.Log("Task 2: Experiment with camouflaged letters, appearing and not appearing, repeated " + task2Repetitions + " times");
                /// ---- Log ----
            }
        }

        // If Key 1 pressed and Task Mode on then start Pilot Experiment
        if (Input.GetKeyDown(KeyCode.Alpha1) && isTaskMode)
        {
            // Remove all reset all
            removeLetterAtDoor();
            removeAllLetters();

            // Task Settings
            currentTask = 1;

            // Task number of repetitions
            taskCount = task1Repetitions;

            // The letter is not camouflaged
            isCamouflaged = false;

            Debug.Log("Task 1 Selected: Not Camouflaged Letters");
        }

        // If Key 2 pressed and Task Mode On then start Experiment 2 
        if (Input.GetKeyDown(KeyCode.Alpha2) && isTaskMode)
        {
            // Remove all reset all
            removeLetterAtDoor();
            removeAllLetters();

            // Task Settings
            currentTask = 2;

            // Task number of repetitions
            taskCount = task2Repetitions;

            // Letters are camouflaged
            isCamouflaged = true;

            // Fill the values for appear the experiment 2 
            fillAppearValues(task2Repetitions, task2LetterAppear);

            Debug.Log("Task 2 Selected: Camouflaged Letters " + task2LetterAppear + "%");
        }
    }

    // Select Camouflaged or not 'C' Key
    public void changeCamouflagedInput()
    {
        // Camouflaged freely if it is not a task mode
        if (Input.GetKeyDown(KeyCode.C) && !isTaskMode)
        {
            isCamouflaged = !isCamouflaged;

            /// ---- Log ----
            Debug.Log("Letter Camouflaged:" + isCamouflaged);
            /// ---- Log ----
        }
    }

    // Select the set of letters
    public void changeSetInput()
    {
        // If Key S pressed then change the current set of letters
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Switch letters set
            if (selectedSet.Equals(letters_set1))
                selectedSet = letters_set2;
            else
                selectedSet = letters_set1;


            /// ---- Log ----
            System.Text.StringBuilder sb = new System.Text.StringBuilder("", 50);
            sb.Append(selectedSet);
            Debug.Log("Selected Set: " + sb.ToString());
            /// ---- Log ----
        }
    }

    // Start Task 'SPACE' Key
    public void startTaskInput()
    {
        // If Key space is pressed then do the different actions for task a non-task
        if (Input.GetKeyDown("space") && (!isTaskMode || (taskCount != 0 && isTaskMode)))
        {
            // The letter is already created?
            if (isLettersCreated)
            {
                // If it is a task mode
                if (isTaskMode)
                {
                    if (taskCount > 0)
                    {
                        // Then decrease task count
                        Debug.Log("Task Iteration " + taskCount + " Finished");
                        taskCount--;
                    }
                    else
                    {
                        // Task finished!
                        currentTask = 0;
                        isAppearing = true;
                        isTaskMode = false;

                        /// ---- Log ----
                        Debug.Log("Task Finished!!");
                        /// ---- Log ----
                    }
                }


                // Store time of the last task
                deltaTime = Time.realtimeSinceStartup * 1000.0f - lastTime;
                Debug.Log("Task Time: " + deltaTime + " milliseconds");

                // Remove letters to start again or stop
                removeAllLetters();
            }
            else
            {
                // Is Letter Shown At door?
                if (!isLetterShownAtDoor)
                {
                    // If not then generate a new letter for the task from the selectedSet
                    letter = generateRandomLetter(selectedSet);

                    /// ---- Log ----
                    Debug.Log("Target Letter: " + letter);
                    /// ---- Log ----

                    // Appear the letter at the door
                    showLetterAtDoor(letter, isCamouflaged);
                }
                else
                {

                    // Remove letter at the door
                    removeLetterAtDoor();

                    // Store time from the start of the task
                    lastTime = Time.realtimeSinceStartup * 1000.0f;

                    if (isTaskMode)
                    {
                        if (currentTask == 2)
                            isAppearing = appear[taskCount - 1];
                        else
                            isAppearing = true;

                        /// ---- Log ----
                        Debug.Log("Task: " + taskCount);
                        Debug.Log("Is Letter Appearing: " + isAppearing);
                        /// ---- Log ----
                    }

                    // Generate all letters at the room
                    generateRandomLetters(letter, isCamouflaged, isAppearing, selectedSet);
                }
            }
        }
    }


    // Change Appearing 'A' Key
    public void changeAppearingInput()
    {
        // if the F is pressed then change FOV
        if (Input.GetKeyDown(KeyCode.A))
        {
            isAppearing = !isAppearing;

            /// ---- Log ----
            Debug.Log("Appearing Is: " + (isAppearing ? "Enabled" : "Disabled"));
            /// ---- Log ----
        }
    }

    /// =========================================================
    /// ============= LETTERS GENERATOR METHODS =================
    /// =========================================================
    /// <summary>
    /// Show Letter to test at door to show it to user
    /// </summary>
    /// <param name="letter">The letter to show</param>
    /// <param name="camouflaged">If it has to be camouflaged or not</param>

    
    public void showLetterAtDoor(char letter, bool camouflaged)
    {
        // Instantiate the letter prefab and put it on the door
        letterDoor = Instantiate(letterPrefab, door.transform.position, door.transform.rotation) as GameObject;
        TextMesh letterDoorText = letterDoor.GetComponent<TextMesh>();

        // If the door is not camouflaged then change of color
        if (!camouflaged)
            letterDoorText.color = targetLetter;

        // Change the text to the desired letter
        letterDoorText.text = letter.ToString();

        // The letter is shown at the door
        isLetterShownAtDoor = true;
    }
    


    /// <summary>
    /// Generate random letters at walls, ceil and at ground
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    /// <param name="appear"></param>
    /// <param name="set"></param>
    public void generateRandomLetters(char letter, bool camouflaged, bool appear, char[] set)
    {
        int position = generateRandomNumber(0, 171);

        generateRandomLettersAtWall(Wall.Left, letter, position, camouflaged, appear, set, new int[] { 28, 29 });
        generateRandomLettersAtWall(Wall.Right, letter, position, camouflaged, appear, set, new int[] { 10, 11 });
        generateRandomLettersAtWall(Wall.Front, letter, position, camouflaged, appear, set, new int[] { -1 });
        generateRandomLettersAtWall(Wall.Back, letter, position, camouflaged, appear, set, new int[] { 10, 11, 12, 16, 17, 18 });
        generateRandomLettersAtCeil(letter, position, camouflaged, appear, set);
        generateRandomLettersAtGround(letter, position, camouflaged, appear, set);

        isLettersCreated = true;
        ///Debug.Log(letterCount + " letters Created!");
    }


    /// <summary>
    /// Generate random letters at the specified wall
    /// </summary>
    /// <param name="wall">Use the Enum Wall to select a specific wall</param>
    /// <param name="letter">The letter that we want to hide around the letters</param>
    /// <param name="atPosition">The position where we want to hide the letter, this could be a random position from 0 to 171</param>
    /// <param name="camouflaged">The letter should be camouflaged?</param>
    /// <param name="appear">The letter should appear?</param>
    /// <param name="set">The set of the letters</param>
    /// <param name="ignore">The letters ignored to appear depending of location</param>
    private void generateRandomLettersAtWall(Wall wall, char letter, int atPosition, bool camouflaged, bool appear, char[] set, int[] ignore)
    {
        // Get the corner top-left start position for the letters
        Vector3 startPosition = getLetterStartPositionAtWall(wall);

        // Get the rotation for the current letter determined by the normal of the wall
        Quaternion rotation = getLetterRotationAtWall(wall);

        int letterCountPerWall = 0;
        for (int i = 0; i < letterColumns; i++)
        {
            for (int j = 0; j < letterRows; j++)
            {
                // Check if the current character should be ignored because probably there is a window or a door over it
                if (!UnityEditor.ArrayUtility.Contains<int>(ignore, letterCountPerWall + 1))
                {
                    // Calculate the new letter position
                    Vector3 letterPosition = getNewLetterPositionAtWall(wall, startPosition, i, j);

                    // Create and Save letter in the list
                    letters.Add(createNewLetter(letterPosition, rotation, letter, atPosition, camouflaged, appear, set));
                }

                // Increment count of letters for this specific wall
                letterCountPerWall++;
            }
        }
    }


    /// <summary>
    /// Generate letters at ground
    /// </summary>
    /// <param name="letter">The letter that we want to hide around the letters</param>
    /// <param name="atPosition">The position where we want to hide the letter, this could be a random position from 0 to 171</param>
    /// <param name="camouflaged">The letter should be camouflaged?</param>
    /// <param name="appear">The letter should appear?</param>
    /// <param name="set">The set of the letters</param>
    private void generateRandomLettersAtGround(char letter, int atPosition, bool camouflaged, bool appear, char[] set)
    {
        generateRandomLettersInACircle(11, ground.transform.position, Vector3.down, letter, atPosition, camouflaged, appear, set, new int[] { -1 });
        generateRandomLettersInACircle(17, ground.transform.position, Vector3.down, letter, atPosition, camouflaged, appear, set, new int[] { 1, 2, 4, 5, 6, 9, 10, 11, 13, 14, 15 });
    }


    /// <summary>
    /// Generate letters at the ceiling
    /// </summary>
    /// <param name="letter">The letter that we want to hide around the letters</param>
    /// <param name="atPosition">The position where we want to hide the letter, this could be a random position from 0 to 171</param>
    /// <param name="camouflaged">The letter should be camouflaged?</param>
    /// <param name="appear">The letter should appear?</param>
    /// <param name="set">The set of the letters</param>
    private void generateRandomLettersAtCeil(char letter, int atPosition, bool camouflaged, bool appear, char[] set)
    {
        generateRandomLettersInACircle(11, ceilling.transform.position, Vector3.up, letter, atPosition, camouflaged, appear, set, new int[] { -1 });
        generateRandomLettersInACircle(17, ceilling.transform.position, Vector3.up, letter, atPosition, camouflaged, appear, set, new int[] { 1, 2, 5, 6, 9, 10, 13, 14 });
    }


    /// <summary>
    /// Generate random letters in a circle
    /// </summary>
    /// <param name="numberOfLetters"></param>
    /// <param name="center"></param>
    /// <param name="upwards"></param>
    /// <param name="letter"></param>
    /// <param name="atPosition"></param>
    /// <param name="camouflaged"></param>
    /// <param name="appear"></param>
    /// <param name="set"></param>
    /// <param name="ignore"></param>
    private void generateRandomLettersInACircle(int numberOfLetters, Vector3 center, Vector3 upwards, char letter, int atPosition, bool camouflaged, bool appear, char[] set, int[] ignore)
    {
        float circumference = letterCircle * numberOfLetters;
        float radius = circumference / (2 * Mathf.PI);
        float angleSpace = 2 * Mathf.PI * letterCircle / circumference;

        for (int i = 0; i < numberOfLetters; i++)
        {
            if (!UnityEditor.ArrayUtility.Contains<int>(ignore, i + 1))
            {
                float x = radius * Mathf.Cos(angleSpace * i);
                float y = radius * Mathf.Sin(angleSpace * i);

                Vector3 position = new Vector3(x, 0.0f, y);
                Vector3 lookPos = Vector3.zero;

                if (upwards == Vector3.down)
                    lookPos = new Vector3(-x, -100, -y);
                else
                    lookPos = new Vector3(x, 100, y);

                Quaternion rotation = Quaternion.LookRotation(lookPos, upwards);


                // Create and Save letter in the list
                letters.Add(createNewLetter(center + position, rotation, letter, atPosition, camouflaged, appear, set));
            }
        }
    }

    /// <summary>
    /// Simply get the new letter position depending of the Wall
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="startPosition"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private Vector3 getNewLetterPositionAtWall(Wall wall, Vector3 startPosition, int i, int j)
    {
        switch (wall)
        {
            case Wall.Left:
                return new Vector3(startPosition.x + i * letterHori,
                                   startPosition.y - j * letterVerti,
                                   startPosition.z);
            case Wall.Right:
                return new Vector3(startPosition.x - i * letterHori,
                                   startPosition.y - j * letterVerti,
                                   startPosition.z);
            case Wall.Back:
                return new Vector3(startPosition.x,
                                   startPosition.y - j * letterHori,
                                   startPosition.z - i * letterVerti);
            case Wall.Front:
                return new Vector3(startPosition.x,
                                   startPosition.y - j * letterHori,
                                   startPosition.z + i * letterVerti);
            default:
                return new Vector3(startPosition.x,
                                   startPosition.y - j * letterVerti,
                                   startPosition.z - i * letterHori);
        }
    }

    /// <summary>
    /// Get letter rotation depending of the wall
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    private Quaternion getLetterRotationAtWall(Wall wall)
    {
        switch (wall)
        {
            case Wall.Left:
                return Quaternion.Euler(0, 180, 0);
            case Wall.Right:
                return Quaternion.Euler(0, 0, 0);
            case Wall.Back:
                return Quaternion.Euler(0, -90, 0);
            case Wall.Front:
                return Quaternion.Euler(0, 90, 0);
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// Get the start position where we will iterate the letters at the certain wall
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    private Vector3 getLetterStartPositionAtWall(Wall wall)
    {
        switch (wall)
        {
            case Wall.Left:
                return new Vector3(leftWall.transform.position.x - leftWall.transform.localScale.x / 2 + leftMargin,
                                   leftWall.transform.position.y + leftWall.transform.localScale.y / 2 - topMargin,
                                   leftWall.transform.position.z);
            case Wall.Right:
                return new Vector3(rightWall.transform.position.x + rightWall.transform.localScale.x / 2 - leftMargin,
                                   rightWall.transform.position.y + rightWall.transform.localScale.y / 2 - topMargin,
                                   rightWall.transform.position.z);
            case Wall.Back:
                return new Vector3(backWall.transform.position.x ,
                                   backWall.transform.position.y + backWall.transform.localScale.y / 2 - topMargin,
                                   backWall.transform.position.z + backWall.transform.localScale.z / 2 - leftMargin);
            case Wall.Front:
                return new Vector3(frontWall.transform.position.x ,
                                   frontWall.transform.position.y + frontWall.transform.localScale.y / 2 - topMargin,
                                   frontWall.transform.position.z - frontWall.transform.localScale.z / 2 + leftMargin);
            default:
                return new Vector3(leftWall.transform.position.x - leftWall.transform.localScale.x / 2 - leftMargin,
                                   leftWall.transform.position.y + leftWall.transform.localScale.y / 2 - topMargin,
                                   leftWall.transform.position.z);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="letter"></param>
    /// <param name="atPosition"></param>
    /// <param name="camouflaged"></param>
    /// <param name="appear"></param>
    /// <param name="set"></param>
    /// <returns></returns>
    private GameObject createNewLetter(Vector3 position, Quaternion rotation, char letter, int atPosition, bool camouflaged, bool appear, char[] set)
    {
        // Instantiate the new letter in the desired position
        GameObject letterGO = Instantiate(letterPrefab, position, rotation) as GameObject;

        // If the count of letters is in the desired position for the hidden letter to appear, and if we want it to appear
        // then continue
        if (letterCount == atPosition && appear)
        {
            // Edit the text and write the letter we want
            letterGO.GetComponent<TextMesh>().text = letter.ToString();

            // If the letter is not camouflaged then 
            if (!camouflaged)
                letterGO.GetComponent<TextMesh>().color = targetLetter;
            else
                letterGO.GetComponent<TextMesh>().color = normalLetter;
        }
        else
            letterGO.GetComponent<TextMesh>().text = generateRandomLetter(set, letter).ToString();

        // Count the total number of letters
        letterCount++;

        return letterGO;
    }

    /// <summary>
    /// Generate a Random Letter without omitting
    /// </summary>
    /// <param name="set">The characters set</param>
    /// <returns>A new generated random letter</returns>
    private char generateRandomLetter(char[] set)
    {
        return set[(int)Mathf.Round(UnityEngine.Random.Range(0.0f, (float)(set.Length - 1)))];
    }

    /// <summary>
    /// Generate a Random Letter
    /// </summary>
    /// <param name="set">The characters set</param>
    /// <param name="omit">The omitted character</param>
    /// <returns>A new generated random letter</returns>
    private char generateRandomLetter(char[] set, char omit)
    {
        char gen;

        // Generate a random letter, repeat if the generated character is the omitted one, this means that
        // we want it to appear just once
        do
        {
            gen = generateRandomLetter(set);

        } while (omit == gen);

        return gen;
    }

    /// <summary>
    /// Generate random number from the range min to max.
    /// </summary>
    /// <param name="min">Minimum Integer</param>
    /// <param name="max">Maximum Integer</param>
    /// <returns>Random Integer</returns>
    private int generateRandomNumber(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// TASK SPECIFIC: For the task where we want to appear the letter a fixed number of times 
    /// in a random order.
    /// </summary>
    /// <param name="task_repetitions">The number of times the task is repeated</param>
    /// <param name="percent">The percentage that the letter appear.</param>
    private void fillAppearValues(int task_repetitions, float percent)
    {
        appear = new bool[task_repetitions];

        // Calculate the number of time appear is true based on the specified percentage
        int appear_repetitions = (int)Mathf.Round((float)task_repetitions * ((float)percent / 100.0f));

        // Populate the appear array with true for appearing and false for not appearing
        do
        {
            int i = 0;

            // It should appear the number of times of appear_repetitions.
            while (i < task_repetitions && appear_repetitions > 0)
            {
                // If the appear has a false value there
                if (!appear[i])
                {
                    // fill in the appear value false or true randomly
                    int b = Mathf.RoundToInt(UnityEngine.Random.Range(0.0f, 1.0f));
                    appear[i] = (b == 0 ? false : true);

                    // If appear is true then substract 1 for the number of appear repetitions left
                    if (appear[i])
                        appear_repetitions--;
                }

                i++;
            }

        } while (appear_repetitions > 0);
    }

    /// <summary>
    /// Remove the letter that appeared in the door
    /// </summary>
    
    
    private void removeLetterAtDoor()
    {
        // If the letter at the door exists, then destroy
        if (letterDoor != null)
            Destroy(letterDoor);

        // The letter is not shown anymore
        isLetterShownAtDoor = false;
    }
    

    /// <summary>
    /// Remove all letters from the walls
    /// </summary>
    private void removeAllLetters()
    {
        // If there are letters on the walls
        if (letters.Count != 0)
        {
            // Remove each letters game objects
            foreach (GameObject go in letters)
                Destroy(go);

            // Reset letters list to 0
            letters.Clear();
        }

        // We don't have any letters on the walls
        letterCount = 0;

        // Letters aren't created
        isLettersCreated = false;
    }
}
