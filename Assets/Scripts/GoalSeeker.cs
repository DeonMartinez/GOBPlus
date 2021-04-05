using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoalSeeker : MonoBehaviour
{
    public Text DisconTxt;
    //public Text HungerTxt;
    //public Text ThirstTxt;
    //public Text SleepTxt;
    public Text StatusTXT;
    public Text PleeTxt;
    
    
    Goal[] mGoals;
    Action[] mActions;
    Action mChangeOverTime;
    const float TICK_LENGTH = 5.0f;

    void Start()
    {
        // my inital motives/goals
        mGoals = new Goal[3];
        mGoals[0] = new Goal("Eat", 4);
        mGoals[1] = new Goal("Sleep", 3);
        mGoals[2] = new Goal("Bathroom", 3);

        // the actions I know how to do
        mActions = new Action[6];
        //eat raw food
        mActions[0] = new Action("consume flesh to gain strength");
        mActions[0].targetGoals.Add(new Goal("Eat", -5f));
        mActions[0].targetGoals.Add(new Goal("Sleep", +1f));
        mActions[0].targetGoals.Add(new Goal("Bathroom", +1f));

        //eat a snack
        mActions[1] = new Action("eat a snack");
        mActions[1].targetGoals.Add(new Goal("Eat", -2f));
        mActions[1].targetGoals.Add(new Goal("Sleep", 0f));
        mActions[1].targetGoals.Add(new Goal("Bathroom", +1f));
        
        //sleep in bed
        mActions[2] = new Action("surrender to my sleep demon and regain power");
        mActions[2].targetGoals.Add(new Goal("Eat", +2f));
        mActions[2].targetGoals.Add(new Goal("Sleep", -6f));
        mActions[2].targetGoals.Add(new Goal("Bathroom", +2f));

        //sleep on sofa
        mActions[3] = new Action("briefly tempt my sleep demon but deny him his prize");
        mActions[3].targetGoals.Add(new Goal("Eat", +1f));
        mActions[3].targetGoals.Add(new Goal("Sleep", -3f));
        mActions[3].targetGoals.Add(new Goal("Bathroom", +1f));

        //drink soda
        mActions[4] = new Action("consume fizzy potions to fend off my sleep demon");
        mActions[4].targetGoals.Add(new Goal("Eat", -1f));
        mActions[4].targetGoals.Add(new Goal("Sleep", -1f));
        mActions[4].targetGoals.Add(new Goal("Bathroom", +3f));

        //go to bathroom
        mActions[5] = new Action("do as the Garrett do");
        mActions[5].targetGoals.Add(new Goal("Eat", 0f));
        mActions[5].targetGoals.Add(new Goal("Sleep", 0f));
        mActions[5].targetGoals.Add(new Goal("Bathroom", -4f));

        // the rate my goals change just as a result of time passing
        mChangeOverTime = new Action("tick");
        mChangeOverTime.targetGoals.Add(new Goal("Eat", +4f));
        mChangeOverTime.targetGoals.Add(new Goal("Sleep", +3f));
        mChangeOverTime.targetGoals.Add(new Goal("Bathroom", +2f));
        PleeTxt.text = "Hit E to do something. Or I shall surely perish";
        StatusTXT.text = "Starting clock. One hour will pass every " + TICK_LENGTH + " seconds. I stand still awating death.";

        Debug.Log("Starting clock. One hour will pass every " + TICK_LENGTH + " seconds. I stand still awating death.");
        InvokeRepeating("Tick", 0f, TICK_LENGTH);


        Debug.Log("Hit E to do something. Or I shall surely perish");
    }

    void Tick()
    {
        // apply change over time
        foreach (Goal goal in mGoals)
        {
            goal.value += mChangeOverTime.GetGoalChange(goal);
            //Debug.Log(mChangeOverTime.GetGoalChange(goal));
            goal.value = Mathf.Max(goal.value, 0);
        }

        // print results
        PrintGoals();
    }

    void PrintGoals()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        Debug.Log(goalString);
        DisconTxt.text = goalString;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PleeTxt.text = "Thanks Buddy";
            Action bestThingToDo = ChooseAction(mActions, mGoals);
            Debug.Log("I think I will " + bestThingToDo.name);

            StatusTXT.text = ("I think I will " + bestThingToDo.name);

            // zhu li do the thing
            foreach (Goal goal in mGoals)
            {
                goal.value += bestThingToDo.GetGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }

            //Debug.Log("-- NEW GOALS --");
            PrintGoals();
        }
    }

    Action ChooseAction(Action[] actions, Goal[] goals)
    {

        // find the action leading to the lowest discontentment
        Action bestAction = null;
        float bestValue = float.PositiveInfinity;

        foreach (Action action in actions)
        {
            float thisValue = Discontentment(action, goals);
            //Debug.Log("Maybe I should " + action.name + ". Resulting discontentment = " + thisValue);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
            }
        }

        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        // keep a running total
        float discontentment = 0f;

        // loop through each goal
        foreach (Goal goal in goals)
        {
            // calculate the new value after the action
            float newValue = goal.value + action.GetGoalChange(goal);
            newValue = Mathf.Max(newValue, 0);

            // get the discontentment of this value
            discontentment += goal.GetDiscontentment(newValue);
        }

        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach (Goal goal in mGoals)
        {
            total += (goal.value * goal.value);
        }
        return total;
    }
}