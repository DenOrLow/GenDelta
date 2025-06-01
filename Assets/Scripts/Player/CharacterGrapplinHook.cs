using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;
using TMPro;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Grapplin Hook")]
    public class CharacterGrpplinHook : CharacterAbility
    {
        public override string HelpBoxText() { return "Grapplin Hook Setup"; }

        [Header("Grappling Hook")]
        public float HookSpeed = 30f;
        public LayerMask GrappleLayerMask;
        public LineRenderer LineRenderer;
        public GameObject HookOrigin;
        public float HookCooldownDuration = 1.5f;

        private float _lastHookTime = -Mathf.Infinity;
        private Vector2 _grapplePoint;
        private bool _grappling;

        private Vector2 _direction;

        // Animation parameters
        protected const string _todoParameterName = "TODO";
        protected int _todoAnimationParameter;

        public Image AbilityImage;          // —сылка на Image заднего фона
        public Sprite AbilityCooldown;
        public Sprite AbilityNormal;
        public Text Cooldown;
        private float _cooldown;
        protected override void Initialization()
        {
            base.Initialization();
            
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (Time.time < _lastHookTime + HookCooldownDuration)
            {
                AbilityImage.sprite = AbilityCooldown;
                _cooldown = Mathf.Clamp(1 - (Time.time - _lastHookTime), 0, 1);
                Cooldown.text = _cooldown.ToString("F2");
            }
            else
            {
                AbilityImage.sprite = AbilityNormal;
                Cooldown.text = "";
            }

            if (_grappling)
            {
                _controller.SetForce(Vector2.zero);

                _direction = (_grapplePoint - (Vector2)transform.position).normalized;
                _controller.SetVerticalForce(_direction.y * HookSpeed);
                _controller.SetHorizontalForce(_direction.x * HookSpeed);

                LineRenderer.SetPosition(0, HookOrigin.transform.position);
                LineRenderer.SetPosition(1, _grapplePoint);

                if (Vector2.Distance(transform.position, _grapplePoint) < 3f)
                {
                    StopGrapple();
                }
            }
        }

        protected override void HandleInput()
        {
            base.HandleInput();

            if (Input.GetMouseButtonDown(1) && Time.time >= _lastHookTime + HookCooldownDuration) // ѕ ћ Ч крюк
            {
                StartGrapple();
            }

            if (Input.GetMouseButtonUp(1))
            {
                StopGrapple();
            }
        }

        private void StartGrapple()
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(HookOrigin.transform.position, mouseWorld - (Vector2)HookOrigin.transform.position, 20f, GrappleLayerMask);

            if (hit.collider != null)
            {
                _grapplePoint = hit.point;
                _grappling = true;
                LineRenderer.enabled = true;

                _lastHookTime = Time.time;
            }
        }

        private void StopGrapple()
        {
            _grappling = false;
            LineRenderer.enabled = false;
        }
     

        
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_todoParameterName, AnimatorControllerParameterType.Bool, out _todoAnimationParameter);
        }

        
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _todoAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters);
        }
    }
}