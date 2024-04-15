# Example of a Document


## Employee

```
Headers:
    Id: "x0042069"
Body: 

Id: "x003809"
hireDate: 03/02/2023

```

You want this "root" level document to be as immutable as possible.
Probably *don't* even put name, email, department, salary, etc. as part of this document.


To "Delete" this employee, maybe something like this:

```
Headers:
    Id: "x0042069"
Body: 
null

```
This tells the consumer of this that they should "delete" this employee.

It is called a "tombstone".

(Kafka specific: This stream can be periodically compacted)

## Augmenting Documents Through Events

This document becomes the "stake in the ground" ("Aggregate Root") for a "thing"
that exists (software item, employee, policy, etc.)

Different services will be responsible for different *operations* on this. 
One service might be responsible for the contact information for an employee, another for their salary, who their manager is, etc. These would be published on different topics (with different visibility/authn&authz).

For example:

```
employee-manager-assigned

id: "x0042069",
managerId: "a0399339"

```

Or

```
employee-email-assigned

id: "x0042069",
"email": "jeff@hypertheory.com"
```


These kind of events *often* have versions attached to them. Usually just incrementing integers (can be timestamps, but clocks are hard).


So, imagine a timeline:

```
employee-position-assigned

id: "x0042069",
v: 1,
department: "DEV"
```

```
employee-email-assigned

id: "x0042069",
v: 2,
department: "CEO"
```


If we produce a message that says we want to terminate an employee, we should
include the version-dependent employee. We can't say:

```
employee-fired

id: "x0042069",
reason: "Bad grooming habits"
```

because maybe this message was produced BEFORE the employee was promoted to CEO.

Instead, you'd do:

```
employee-fired

employee: {
    id: "x0042069",
    version: 1
}
```

Remember, the owner of the employee data is the **only service that can change the data**. It can say "Nope, there is a conflict. That isn't the *same* employee".


## Summary

The beauty of this is that each service/bounded-context/subdomain can use event-sourcing to create JUST what they want/need. 

