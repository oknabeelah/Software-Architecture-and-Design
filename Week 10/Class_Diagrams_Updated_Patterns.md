## Creational Design Pattern

**Task:** we have been given the class dagram for research activity management with a service that creates 3 different types of research activities.
Currently, the service is responsible for creating and saving the research activity.

It uses a large if else to switch between all three types, this can be optimised to use the abstract reseach activity type because as the number of activity type increases, the service has to loop through all the options when creating/saving.

the responsibility of creating/saving can be shifted to a factory method that defines an interface for creating objects but lets subclasses decide which object to instantiate.

the service can call the factory method, or become the factory which produces an abstract activity.
concrete activities then ovveride the abstract parent


# Factory Method Pattern
```mermaid
---
title: Research Activity Management - Factory method
---

classDiagram
    ResearchActivityService --> Research Activity : creates abstract activity
    ResearchActivity --> ConferenceActivity : ovverides and creates concrete activity
    ResearchActivity --> PublicationActivity : ovverides and creates concrete activity
    ResearchActivity --> TrainingActivity : ovverides and creates concrete activity

    class ResearchActivityService {
        +CreateActivity(activity): ResearchActivity
        +SaveActivity(activity)
    }

    class ResearchActivity["abstract ResearchActivity"] {
        <<interface>>
        +ActivityId : int
        +Title : string
        +ActivityDate : DateTime
        +CreateNewActivity(ActivityId, Title, ActivityDate) : new ResearchActivity
    }

    class ConferenceActivity["ConferenceActivity : ResearchActivity"] {
        +ConferenceName : string
        +Location : string
        +PresentationTitle : string
        +CreateNewActivity() ovveride : new ConferenceActivity
    }

    class PublicationActivity["PublicationActivity : ResearchActivity"] {
        +JournalName : string
        +PublicationDate : DateTime
        +CreateNewActivity() override : new PublicationActivity
    }

    class TrainingActivity["TrainingActivity : ResearchActivity "] {
        +TrainingProvider : string
        +Duration : int
        +CreateNewActivity() override : new TrainingActivity
    }

```

## Structural Design Pattern

**Task:** we have been given different external notification providers and need to unify them on a single interface without changing their individual implementation
this will be done using an adapter design pattern, which is a structural pattern that acts as a bridge between two incompatible interfaces
- class adapter (using inheritance)
- object adapter (using composition)
using the object adapter, it holds an instance of the adaptee object and provides the required interface to the client

```mermaid
---
title: Notification and Reminder Management - Object Adapter design pattern
---

classDiagram
    NotificationService --> NotificationAdapter : calls with type
    NotificationAdapter --> EmailNotificationProvider : uses requested external provider
    NotificationAdapter --> SMSNotificationProvider : uses requested external provider

    

    class NotificationService {
        +SendNotification(type, target, message)
    }

    class NotificationAdapter {
        +NotificationType : string
        +SendEmail(target, message) : EmailNotificationProvider.SendEmail()
        +DispatchSMS(target, message) : SMSNotificationProvider.DispatchSMS()
    }

    class EmailNotificationProvider {
        +SendEmail(recipient, message)
    }

    class SMSNotificationProvider {
        +DispatchSMS(mobileNumber, content)
    }

```

## Behavioural Design Pattern

**Task:** simplify the flow of notification whenever a change is made to milestone
Behavioral design patterns focus on how objects communicate and collaborate to manage responsibilities effectively.
They improve flexibility and simplify complex control flows within a system.

Observer Design Pattern is a behavioral pattern that creates a one-to-many relationship between a subject and its observers. When the subject's state changes, all dependent observers are notified and updated automatically, ensuring synchronized communication.

```mermaid
---
title: Milestone Management - Behavioural observer design pattern
---
classDiagram
    Subject --> MilestoneService : 
    MilestoneService --> Observer : notify observers -> NotifyObservers(observers)
    Observer --> NotificationService : sends alert
    Observer --> DashboardService : updates dashboard
    Observer --> ReminderService : updates reminder

    class Subject {
        <<interface>>
        -observers : List
        -subjectState: State
        +GetState()
        +SetState()
        +AddObserver()
        +RemoveObserver()
        +NotifyObservers() 
    }
    
    class MilestoneService["MilestoneService : Subject"] {
        +UpdateMilestone()
        +CompleteMilestone()
    }

    class Observer {
        <<interface>>
        +Update()
    }

    class NotificationService["NotificationService : Observer"] {
        -observerState: State
        +Update() override : +SendAlert()
    }

    class DashboardService["DashboardService : Observer"] {
        -observerState: State
        +Update() override : +UpdateDashboard()
    }

    class ReminderService["ReminderService : Observer"] {
        -observerState: State
        +Update() override : +UpdateReminder()
    }

```