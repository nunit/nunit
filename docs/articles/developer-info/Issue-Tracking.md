We use the GitHub issue tracker to keep track of our ongoing work. We supplement GitHub's native issue handling by using [ZenHub](http://www.zenhub.com), which provides a number of additional features.

### Workflow
We use a [ZenHub](http://www.zenhub.com) Board with a standard set of pipelines to track the flow of work in each repo. Our standard set of pipeline columns are slightly different from the ZenHub default:

* **New Issues** All issues start as new. We like to review them as quickly as possible. Some issues can be immediately closed while others need to be categorized for further work. We usually assign at least an _is_ and a _priority_ label (see below) to every issue and then move it to either the **Discussion** or the **Backlog** column.

* **Discussion** These  are items that require some discussion, either about whether we want to do them or how they should be implemented. Some items here may require confirmation or design as well.

* **Backlog** This is our product backlog consisting of all the issues we want to get done now or in the future. In the past, we have often put things we didn't really want to do in our Backlog, assigning them a low priority. While this avoids having to say no to the requester, it doesn't do them any real service. Putting an issue into the Backlog should mean that it will eventually make it to a release.

* **To Do** These are things we are ready to get working on right now and represent a suggestion to developers as to what to choose to work on next. Most but not all of them will be scheduled for the next milestone. This is a good place to keep issues that are blocking other work, so as to encourage developers to get the issue done.

  **Note:** Since the number of people actively working on a given project varies, we can't establish a fixed number of items to keep in this column. The project lead should try to keep it to a reasonable size, generally no more than twice the number of active developers. If it seems to be growing too rapidly, some of the issues should be moved to the backlog.

* **In Progress** These are issues that somebody is already working on. That person should be shown as assigned to the issue and would normally be the one to move the issue into this column.

* **Done** When all work is done and the person doing it feels it is ready to be merged, the issue should be moved to this column. It remains there while comments are discussed and small changes are made. If the comments will lead to significant rework, then move the issue back to In Progress. 

* **Closed** Issues go here automatically when they are closed.

### Issue Assignment

Normally, committers self-assign items they want to work on. Please don't assign something to yourself and then let it sit. As a matter of courtesy, nobody else will start working on something that is assigned to you, so the result is that the work won't get done.

GitHub won't let non-committers assign issues to themselves (or anyone) so if a contributor wants to work on an issue, they should post a comment to that effect. One of the committers will make the assignment. In many cases, the assignment will be shared between the contributor and a committer who is providing mentoring for them.

If an issue that is assigned to you then you should do what needs to be done. For example, if it's in the discussion column, then it's up to you to make sure the discussion happens and progresses to a conclusion. If it's marked _confirm_ then you should confirm it. If it's marked _design_ then you should do the design and get it reviewed. And, of course, if it's ready for implementation you should do that!

### Issue Labels
We try to use a standard set of labels across all the NUnit repositories. In some cases, an individual project may not yet have been converted to use these labels, but we expect to do so soon.

That said, don't stress about whether something is a bug or an enhancement, normal versus low priority, etc. Just pick one. If things change later, the label can be changed as well. This is only intended to help us organize a relatively large number of issues, not to give us extra work.

#### What it is
Labels starting with **is:** indicate the nature of the issue. Only one should be used, based on the judgment of the committer who assigns the label. If there is no **is:** label, then we presumably don't know what the item is and should not be working on it!
* **is:bug** Something that isn't working as designed.
* **is:docs** Solely pertaining to the documentation or sample code.
* **is:enhancement** An addition or improvement to an existing feature.
* **is:feature** An entirely new feature.
* **is:idea** An idea about something we might do. We discuss these until they are either dropped or turned into a feature or enhancement we can work on.
* **is:question** Just a question - we discourage these as issues but they do happen.
* **is:build** Something to do with how we build the software, scripts, etc.
* **is:refactor** What it says: refactoring that is needed.

#### Priority
Labels starting with **pri:** indicate the priority of an issue. Pick just one, please. Priority may, of course, change over time, as items become more or less important to us. If no priority is assigned, we shouldn't be working on it.
* **pri:critical** Should only apply to bugs, which need to be fixed immediately, dropping everything else. At times, we will even speed up the release cycle due to a critical bug.
* **pri:high** High priority - implement as soon as possible.
* **pri:normal** Standard priority - implement when we can.
* **pri:low** Low priority - implement later or not at all.

#### PRs and issues that need attention
The goal with these is to keep team members from having to do tedious rescanning to figure out the state of each long-lived PR or issue as well as increasing awareness.
- **awaiting:contributor**  
  Blocked until the contributor responds to the team's request for changes.
- **awaiting:team**  
  Blocked until a team member responds to a question or problem.
- **awaiting:discussion**  
  Blocked until the team comes to consensus on a design question.
- **awaiting:review**  
  Blocked until an additional team member approves or requests changes.

#### Close Reason
Labels starting with **closed:** indicate the status of the bug at closing and should only appear on closed bugs. Please remember to apply one of these when closing a bug as it makes it easier to review the list of closed bugs without opening each one to see what the disposition was.
* **closed:done** The work called for is done, i.e. the bug is fixed or the feature/enhancement is implemented.
* **closed:duplicate** The issue is a duplicate of one that we are working. A comment should indicate the issue number.
* **closed:notabug** The issue (generally a bug) is not valid or the feature already exists. There should be an explanatory comment.
* **closed:norepro** While the issue (generally a bug) may exist on the user's system, we have tried and are unable to reproduce it. If somebody later figures out a repro, the issue can be reopened.
* **closed:wontfix** The issue is possibly valid but we don't intend to implement it. It may be out of scope for the project or inconsistent with the values and priorities of the project. There should be an explanatory comment.

#### Other Labels
* **confirm** Somebody should verify that the issue actually exists and then remove the label. In some cases, a bug may have been reported against an older version of NUnit and needs to be checked out using the current code.
* **blocked** The issue cannot be worked on until something else happens, external to the project. There should be a comment on the issue indicating what that something is.
* **design** Some design decisions need to be made before this can really be worked on. Sometimes this label may be applied before anything happens and other times the work may have started but reached a point where design decisions need to be made involving others in the team.
* **up-for-grabs** Indicates a backlog issue ready for implementation where we would love for the wider community to jump in and be assigned! Whoever adds this label should couple it with a comment suggesting what code to look at and a general approach to working the issue.
* **easyfix** Indicates an issue that is expected to take no more than a few nights' work.

### Epics
Where appropriate, we make use of the [ZenHub](http://www.zenhub.com) **Epic** feature to create issues that include a number of sub-issues. For example, when we split the original NUnit repository into separate framework and engine repositories, we created an epic that included a number of separate issues that had to be completed in order to accomplish the split smoothly.

### Milestones
We use GitHub Milestones to represent future releases. Generally milestones are created by the project or team leader, who also decides what issues to place into the milestone. Our practice is to only schedule a limited number of key features in advance for each milestone, rather than trying to "fill" the milestone with the amount of work we think can be completed. Other work is only added to the next milestone after it has been completed.

We do it this way because the amount of time developers have to spend on our projects can vary considerably over the course of time. We never know how much work will be done in a given period. Since we can't predict both the timing and the content of each release, we have chosen to hold the time constant and vary the content.
