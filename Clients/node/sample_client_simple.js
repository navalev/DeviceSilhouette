﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
var silhouetteClient = require('./silhouette-client');

var my_state = {
	Xaxis: 0,
	Yaxis: 0,
	Zaxis: 0
};

var metadata = { 
	"Origin" : "sensor"
};

/*
** This function gets called when the cloud service sends an update to the state.
** The device should update its state accordingly.
*/

function C2D_updateState(state)
{
  // TODO: should I check what is inside the state? 
  // - What if heaterOn has changed? How should I detect to take action?
  // - What if the cloud service wants to set the temperature? It makes no sense e.g. for a sensor.
  // - Do we get all the properties on update or just the ones changed? 
  console.log("in C2D_updateState; new state:");
  console.dir(state)
  my_state = state;
}

/*
** This function gets called when the cloud service wants to know the latest state.
** The device should return its latest state.
*/

function C2D_getState()
{
  console.log("in C2D_getState");
  silhouette.updateState(my_state);
}

/*
** Create the Silhouette client
*/
var deviceID = "device1";
var connectionString = process.env.Silhouette_DeviceIotHubConnectionString

var silhouette = silhouetteClient.create('iothub', {
  connectionString
});

silhouette.on('C2D_updateState', C2D_updateState);
silhouette.on('C2D_getState', C2D_getState);
//silhouette.on('error', error_handle);

/*
** This could work in whatever way you want. We will just set a timer.
*/

setInterval(doWork, 10*1000);

function doWork()
{
  // console.dir(silhouette);
  // Send our new state to the cloud service
  console.log('sending updated state:');
  console.dir(my_state);
  silhouette.updateState(metadata, my_state, deviceID);
  ++my_state.Xaxis;
  
  // get the state from the cloud service
  //console.log('sending get state');
  //check_state = silhouette.getState();
  //console.log(check_state);
  // TODO: we can use the native client to do other stuff
  // silhouette.client.on('someEvent', doSomething)
}

