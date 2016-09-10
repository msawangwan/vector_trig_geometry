using UnityEngine;

public class Oscillator {
    public float Amp { get; private set; }
    public float Period { get; private set; }

    public Oscillator  () {
        Amp = 0.1f;
        Period = 0.1f;
    }

    public Oscillator ( float amp, float period ) {
        Amp = amp;
        Period = period;
    }

    public void Oscillate ( Transform myTransform, Vector3 startingPos ) {
        myTransform.position = startingPos + Vector3.up * ( Amp *  Mathf.Sin ( Time.timeSinceLevelLoad / Period ) );
    }
}
