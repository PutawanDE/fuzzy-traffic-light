using UnityEngine;

public enum linguisticVar { High, Medium, Low }

public class TrafficLightFuzzyLogic : MonoBehaviour
{
    [System.Serializable]
    public class FuzzySets
    {
        public AnimationCurve high;
        public AnimationCurve medium;
        public AnimationCurve low;
    }

    [System.Serializable]
    public struct Modes
    {
        public float high;
        public float medium;
        public float low;
    }

    // (queueingVehicles, priority ; greenLightDuration)
    [System.Serializable]
    public class Rule
    {
        public linguisticVar queueingVehicles;
        public linguisticVar priority;
        public linguisticVar greenLightDuration;
    }

    [SerializeField]
    private FuzzySets queueingVehiclesSet;
    [SerializeField]
    private FuzzySets prioritySet;

    [SerializeField]
    private FuzzySets greenLightDurationSet;

    [SerializeField]
    private Modes greenLighDurationSetModes;

    [SerializeField, NonReorderable]
    private Rule[] rules;

    // Intuition fuzzification -> Mandani Model -> Simplified centroid defuzzification
    public float Evaluate(int vehicleCount, int priority)
    {
        float totalFiringStrengths = 0f;
        float totalFiringStrengthMulMode = 0f;

        // Fuzzify using Animation curve evaluation
        float queueingHighMembershipVal = queueingVehiclesSet.high.Evaluate(vehicleCount);
        float queueingMedMembershipVal = queueingVehiclesSet.medium.Evaluate(vehicleCount);
        float queueingLowMembershipVal = queueingVehiclesSet.low.Evaluate(vehicleCount);

        float priorityHighMembershipVal = prioritySet.high.Evaluate(priority);
        float priorityMedMembershipVal = prioritySet.medium.Evaluate(priority);
        float priorityLowMembershipVal = prioritySet.low.Evaluate(priority);

        for (int i = 0; i < rules.Length; i++)
        {
            float queueingMembershipVal = 0f;
            float priorityMembershipVal = 0f;

            float firingStrength = 0f;
            float outputSetMode = 0f;

            if (rules[i].queueingVehicles == linguisticVar.High)
            {
                queueingMembershipVal = queueingHighMembershipVal;
            }
            else if (rules[i].queueingVehicles == linguisticVar.Medium)
            {
                queueingMembershipVal = queueingMedMembershipVal;
            }
            else if (rules[i].queueingVehicles == linguisticVar.Low)
            {
                queueingMembershipVal = queueingLowMembershipVal;
            }

            if (rules[i].priority == linguisticVar.High)
            {
                priorityMembershipVal = priorityHighMembershipVal;
            }
            else if (rules[i].priority == linguisticVar.Medium)
            {
                priorityMembershipVal = priorityMedMembershipVal;
            }
            else if (rules[i].priority == linguisticVar.Low)
            {
                priorityMembershipVal = priorityLowMembershipVal;
            }

            // Mandani Implication
            firingStrength = Mathf.Min(queueingMembershipVal, priorityMembershipVal);

            // Do not fire
            if(firingStrength <= 0f) continue;

            // Find mode of the output set
            if (rules[i].greenLightDuration == linguisticVar.High)
            {
                outputSetMode = greenLighDurationSetModes.high;
            }
            else if (rules[i].greenLightDuration == linguisticVar.Medium)
            {
                outputSetMode = greenLighDurationSetModes.medium;
            }
            else if (rules[i].greenLightDuration == linguisticVar.Low)
            {
                outputSetMode = greenLighDurationSetModes.low;
            }

            // Calculations for defuzzification
            totalFiringStrengthMulMode += firingStrength * outputSetMode;
            totalFiringStrengths += firingStrength;
        }

        // Continue defuzzifying
        float result = 0f;
        if(totalFiringStrengths > 0f) 
            result = totalFiringStrengthMulMode / totalFiringStrengths;
        
        return result;
    }
}
