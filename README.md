# README #

![Unity Version](https://img.shields.io/badge/Unity%20Version-4.6-orange.svg)

**Reh@Panel** (formerly RehabNet CP) acts as a device router, bridging a large number of tracking devices and other hardware with the RehabNet Training Games for the patient to interact with. Reh@Panel implements the communication protocols in a client/server architecture. Native device support for:

**Electrophysiological Data:**

* Emotiv EPOC neuro-headset is intergrated for acquiring raw EEG data, gyroscope data, facial expressions and Emotiv’s Expressiv™, Cognitiv™ and Affectiv™ suite.
* Neurosky EEG headset is supported for raw EEG acquisition and eSense™ meters of attention and meditation.
* Myoelectric orthosis mPower 1000 (Myomo Inc, Boston, USA) is supported, providing 2 EMG channels and adjustable levels of assistance.
* Bitalino a biosignal acquisition device supporting sensors for electrocardiography (ECG), electromyography (EMG), electrodermal activity (EDA), accelerometer, and ambient light.
* OpenBCI v1 an open source brain-computer interface platform for electrophysiological signal acquisition.

**Kinematics:**

* Microsoft Kinect v1 is natively supported either by the Microsoft or OpenNI drivers.
* Microsoft Kinect v2 through Kinect v2 SDK.
* Nintendo Wii.
* Leap Motion.

**Head tracking**

* Oculus Rift.
* Vuzix iWear.
* faceAPI software with head and face tracking algorithms.

**Eye tracking**

* Tobii T120.
* Tobii EyeX.
* Eye-Tribe.
 
Extended device support is achieved via a custom UDP protocol used for bridging with:

Android app (see [RehaMote](https://bitbucket.org/neurorehablab/rehamote)) running on smartphones for sending sensor data
Analysis and Tracking System (AnTS)
VRPN and OSC protocols are supported for the connection with any device (e.g. Vicon’s tracking, 5DT data gloves) or software supporting it (e.g. OpenViBE BCI software)
 
Reh@Panel performs data filtering, smoothing, translation and emulation on these data. In addition, logging of synchronized data in XML or CSV format is configurable from all the acquisition devices as well as game events for offline analysis. Finally, the CP allows to preview the translated avatar movements from the sensors, allowing to re-adjust parameters in real-time.

![Reh@Panel](https://lh5.googleusercontent.com/-fHhZm9NIFwc/U0Z3GbgtmJI/AAAAAAAABIM/TiZYbttpco0/s720/kinect1.png)

## Reference: ##
Athanasios Vourvopoulos, Ana Lucia Faria, Monica S Cameirao, Sergi Bermudez i Badia (2013)  RehabNet : A distributed architecture for motor and cognitive neuro-rehabilitation In: 2013 IEEE 15th International Conference on e-Health Networking, Applications Services (Healthcom) 454-459.