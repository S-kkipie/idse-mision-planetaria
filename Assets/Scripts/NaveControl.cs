using UnityEngine;

// ODYSSEY-01: propulsion con Space y giro con A/D (Labs 04-08), mas combustible.
[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class NaveControl : MonoBehaviour
{
    public float fuerzaPropulsion = 1000f;
    public float velocidadRotacion = 100f;
    public float combustibleMax = 100f;
    public float consumoPorSegundo = 10f;

    public float Combustible { get; private set; }

    Rigidbody rb;
    AudioSource motor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        motor = GetComponent<AudioSource>();
        Combustible = combustibleMax;
    }

    void Update()
    {
        Propulsion();
        Rotacion();
    }

    void Propulsion()
    {
        if (Input.GetKey(KeyCode.Space) && Combustible > 0f)
        {
            rb.AddRelativeForce(Vector3.up * fuerzaPropulsion * Time.deltaTime);
            Combustible = Mathf.Max(0f, Combustible - consumoPorSegundo * Time.deltaTime);
            if (!motor.isPlaying) motor.Play();
        }
        else if (motor.isPlaying)
        {
            motor.Stop();
        }
    }

    void Rotacion()
    {
        rb.freezeRotation = true; // control manual mientras giramos
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);
        else if (Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.back * velocidadRotacion * Time.deltaTime);
        rb.freezeRotation = false;
    }

    public void Recargar(float cantidad)
    {
        Combustible = Mathf.Min(combustibleMax, Combustible + cantidad);
    }
}
