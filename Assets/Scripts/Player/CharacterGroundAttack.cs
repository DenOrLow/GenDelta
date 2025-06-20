using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
    /// Add this class to a Character and it'll be able to dive by pressing down + dash button, pounding the ground in the process.
    /// Animator parameters : Diving (bool)
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Ground Attack")]
    public class CharacterGroundAttack : CharacterAbility
    {
        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        public override string HelpBoxText() { return "This component allows your character to dive (by pressing the dash button + the down direction while in the air). Here you can define how much the camera should shake on impact, and how fast the dive should be."; }

        /// Shake parameters : intensity, duration (in seconds) and decay
        [Tooltip("Shake parameters : intensity, duration (in seconds) and decay")]
        public Vector3 ShakeParameters = new Vector3(1.5f, 0.5f, 1f);
        /// the vertical acceleration applied when diving
        [Tooltip("the vertical acceleration applied when diving")]
        public float DiveAcceleration = 2f;

        // animation parameters
        protected const string _divingAnimationParameterName = "Diving";
        protected int _divingAnimationParameter;
        protected Coroutine _coroutine;

        [Tooltip("the DamageOnTouch object to activate when dashing (usually placed under the Character's model, will require a Collider2D of some form, set to trigger")]
        public DamageOnTouch TargetDamageOnTouch;

        public Image AbilityImage;          // ������ �� Image ������� ����
        public Sprite AbilityCooldown;
        public Sprite AbilityNormal;
        public Text Cooldown;
        private float _cooldown;

        private float _lastAttackTime = -Mathf.Infinity;
        public float AttackCooldownDuration = 5f;
        /// <summary>
        /// Every frame, we check input to see if we should dive
        /// </summary>
        protected override void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Z) && Time.time >= _lastAttackTime + AttackCooldownDuration)
            {
                // we start the dive coroutine
                InitiateDive();
                TargetDamageOnTouch?.gameObject.SetActive(true);
            }

            if (Time.time < _lastAttackTime + AttackCooldownDuration)
            {
                AbilityImage.sprite = AbilityCooldown;
                _cooldown = Mathf.Clamp(AttackCooldownDuration - (Time.time - _lastAttackTime), 0, AttackCooldownDuration);
                Cooldown.text = _cooldown.ToString("F2");
            }
            else
            {
                AbilityImage.sprite = AbilityNormal;
                Cooldown.text = "";
            }


        }

        public virtual void InitiateDive()
        {
            if (!AbilityAuthorized // if the ability is not permitted
                 || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if it's gripping
                 || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not under normal conditions
            {
                // we do nothing and exit
                return;
            }
            // we start the dive coroutine
            _coroutine = StartCoroutine(Dive());
        }

        /// <summary>
        /// Coroutine used to make the player dive vertically
        /// </summary>
        protected virtual IEnumerator Dive()
        {
            // we start our sounds
            PlayAbilityStartFeedbacks();
            MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Dive, MMCharacterEvent.Moments.Start);

            // we make sure collisions are on
            _controller.CollisionsOn();
            // we set our current state to Diving
            _movement.ChangeState(CharacterStates.MovementStates.Diving);

            // while the player is not grounded, we force it to go down fast
            while (!_controller.State.IsGrounded)
            {
                if ((_movement.CurrentState == CharacterStates.MovementStates.Gripping)
                     || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
                     || (_movement.CurrentState == CharacterStates.MovementStates.LedgeClimbing))
                {
                    StopCoroutine(_coroutine);
                    yield break;
                }

                _controller.SetVerticalForce(-Mathf.Abs(_controller.Parameters.Gravity) * DiveAcceleration);
                yield return null; //go to next frame
            }
            // once the player is grounded, we shake the camera, and restore the diving state to false
            if (_sceneCamera != null)
            {
                _sceneCamera.Shake(ShakeParameters);
            }

            // we play our exit sound
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
            MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Dive, MMCharacterEvent.Moments.End);

            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _lastAttackTime = Time.time;
            TargetDamageOnTouch?.gameObject.SetActive(false);
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_divingAnimationParameterName, AnimatorControllerParameterType.Bool, out _divingAnimationParameter);
        }

        /// <summary>
        /// Sends the current state of the Diving state to the character's animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _divingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Diving), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}