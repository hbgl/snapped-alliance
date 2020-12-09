using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace Assets.Scripts
{
    public class PathMover
    {
        private static float RotationSpeed = 500.0f;
        private static float MoveSpeed = 6.0f;

        public static IEnumerator Move(GameObject thing, List<Vector3> points, Action callback)
        {
            var pointsLength = points.Count;
            if (pointsLength <= 1)
            {
                yield return null;
            }
            var transform = thing.transform;
            var animator = thing.GetComponent<Animator>();

            var state = State.None;

            Vector3 current;
            for (var i = 1; i < pointsLength; i++)
            {
                current = points[i];
                for (; ; )
                {
                    var singleStep = PathMover.RotationSpeed * Time.deltaTime;
                    var targetDirection = (current - transform.position).normalized;
                    var targetRotation = Quaternion.LookRotation(targetDirection);
                    var angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
                    if (angleDifference <= 0.1f)
                    {
                        break;
                    }
                    if (state == State.Move)
                    {
                        animator.SetFloat("MoveSpeed", 0.0f);
                    }
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
                    state = State.Rotate;
                    yield return null;
                }

                var start = transform.position;
                var totalDistance = Vector3.Distance(start, current);
                for (; ; )
                {
                    var currentDistance = Vector3.Distance(start, transform.position);
                    if (Mathf.Abs(currentDistance - totalDistance) <= 0.01f)
                    {
                        break;
                    }
                    if (state != State.Move)
                    {
                        state = State.Move;
                        animator.SetFloat("MoveSpeed", 1.0f);
                    }
                    var step = PathMover.MoveSpeed * Time.deltaTime;
                    var newPosition = Vector3.MoveTowards(transform.position, current, step);
                    transform.position = newPosition;
                    yield return null;
                }
            }

            if (state == State.Move)
            {
                animator.SetFloat("MoveSpeed", 0.0f);
            }
        }

        private enum State
        {
            None,
            Rotate,
            Move,
        }
    }
}