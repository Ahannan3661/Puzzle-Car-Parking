﻿using System;
using UnityEngine;
using PathCreation;
     // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public abstract class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;
        private Camera _camera;
        public bool moveCar,reverse;

        private void Awake()
        {
            FindMyPath();
        }

        protected virtual void Start()
        {
            _camera = Camera.main;
            // if (pathCreator != null)
            // {
            //     // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            //     pathCreator.pathUpdated += OnPathChanged;
            // }
        }

        void Update()
        {
            SelectCar();
            MoveTheCar(moveCar);
        }

        private void SelectCar()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray,out var hit))
                {
                    if (hit.collider.CompareTag("Car"))
                    {
                        if (hit.collider.gameObject == gameObject)
                        {
                            moveCar = true;
                            GameManager.gameManagerInstance.numberOfMoves--;
                        }
                    }
                }
            }
        }
        private void MoveTheCar( bool move)
        {
            if (pathCreator != null && move)
            {
                float distance = pathCreator.path.GetClosestDistanceAlongPath(transform.position);

                if (!reverse)
                    distanceTravelled += speed * Time.deltaTime;
                else
                    distanceTravelled -= speed * Time.deltaTime;
                
                
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                if (distance >= pathCreator.path.length && !reverse) // means reached to end of the road
                {
                    moveCar = false;
                    reverse = true;
                    GameManager.gameManagerInstance.countTracks--;
                    GameManager.gameManagerInstance.Victory();
                    GameManager.gameManagerInstance.Lose();
                }
                
                if (distance <= 0f && reverse) // means the car will back to its initial pos
                {
                    moveCar = false;
                    reverse = false;
                    
                    GameManager.gameManagerInstance.Victory();
                    GameManager.gameManagerInstance.Lose();
                }
            }
        }
        
        
        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        // void OnPathChanged()
        // {
        //     distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        // }

        private void FindMyPath()
        {
            var paths = GameObject.FindGameObjectsWithTag("path");

            string pathName = "Path_" + gameObject.name;

            foreach (var path in paths)
            {
                if (path.name.Equals(pathName))
                {
                    pathCreator = path.GetComponent<PathCreator>();
                    break;
                }
            }
        }
        
    }


  
