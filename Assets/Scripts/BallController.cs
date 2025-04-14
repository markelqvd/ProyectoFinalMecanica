using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Configuración de Impulso")]
    public float impulseMultiplier = 5f;  // Factor para escalar la fuerza del impulso
    public float maxImpulse = 15f;        // Impulso máximo permitido

    [Header("Trajectoria")]
    public LineRenderer trajectoryLine;
    public int numTrajectoryPoints = 30;  // Número de puntos para la trayectoria proyectada
    public float timeStep = 0.1f;         // Intervalo de tiempo entre puntos

    private Rigidbody2D rb;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private bool isDragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ajusta el angular drag para reducir la rotación excesiva
        rb.angularDrag = 2f;

        if (trajectoryLine != null)
            trajectoryLine.positionCount = numTrajectoryPoints;
    }

    // Evento cuando se pulsa el botón del ratón sobre la pelota
    private void OnMouseDown()
    {
        // Permitir arrancar solo si la pelota está casi en reposo
        if (rb.velocity.magnitude < 0.1f)
        {
            isDragging = true;
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    // Evento mientras se arrastra con el ratón
    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector2 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startPoint - currentPoint;
            if (dragVector.magnitude > maxImpulse)
                dragVector = dragVector.normalized * maxImpulse;

            DrawTrajectory(dragVector);
        }
    }

    // Evento al soltar el botón del ratón
    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            if (trajectoryLine != null)
                trajectoryLine.positionCount = 0;

            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startPoint - endPoint;
            if (dragVector.magnitude > maxImpulse)
                dragVector = dragVector.normalized * maxImpulse;

            rb.AddForce(dragVector * impulseMultiplier, ForceMode2D.Impulse);

            // Aquí puedes activar efectos si se alcanza cierta velocidad
            // CheckAndActivateEffects(rb.velocity.magnitude);
        }
    }

    // Método para dibujar la trayectoria proyectada usando la física (gravedad incluida)
    void DrawTrajectory(Vector2 impulse)
    {
        if (trajectoryLine == null)
            return;

        Vector2 startingPos = transform.position;
        Vector2 startingVel = impulse * impulseMultiplier / rb.mass;

        for (int i = 0; i < numTrajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector2 pos = startingPos + startingVel * t + 0.5f * Physics2D.gravity * t * t;
            trajectoryLine.SetPosition(i, pos);
        }
    }

    // Limita la velocidad angular para evitar giros indefinidos
    void FixedUpdate()
    {
        float maxAngularVelocity = 200f;  // Valor máximo deseado (puedes ajustar este valor)
        if (Mathf.Abs(rb.angularVelocity) > maxAngularVelocity)
        {
            rb.angularVelocity = Mathf.Sign(rb.angularVelocity) * maxAngularVelocity;
        }
    }

    /*
    // Ejemplo opcional para activar efectos cuando se alcanza una velocidad mínima
    void CheckAndActivateEffects(float speed)
    {
        float thresholdSpeed = 10f;
        if (speed >= thresholdSpeed)
        {
            Debug.Log("Efecto de velocidad activado");
        }
    }
    */
}
