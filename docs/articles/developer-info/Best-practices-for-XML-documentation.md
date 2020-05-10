XML documentation has a significant impact on the end user's experience. Unlike other parts of the code, XML comments (as well as exception messages) are user-facing and highly visible. They can save valuable time and even make the difference for an end user between a confusing debugging session and no debugging needed in the first place.

Polished and informative XML documentation provides a noticeably positive experience.


### Sentences

Always end sentences with proper punctuation, typically a period.

If there aren't enough words to form a sentence, it's likely that they are redundant. Try to add enough context to make a sentence to avoid anemic documentation. Rather than `The comparer`, say `Used to compare the <paramref name="items"/>.` The added clarity is desirable.


### `<see>`, `<paramref>` and `<typeparamref>`

Use `<see cref="TypeOrMember"/>`, `<paramref name="parameterName"/>` and `<typeparamref name="T"/>` whenever possible.
The compiler checks the validity of the names and overloads. Future refactoring can be done with confidence that you aren't leaving the docs out of date.

This also provides a seamless experience by creating links in the Object Browser and intellisense and colorizes intellisense. Consider using `<see langword="null"/>` for keywords to maintain the seamlessness.


### `<summary>`

Add a `<summary>` tag for each public type and member.

However, prefer to give each type and member a really communicative name. If those names end up covering all the user could need to know, remove the `<summary>` tag (if this does not cause a build warning) to avoid super redundant documentation. If they do not, add relevant details.


### `<param>` and `<typeparam>`

Add a `<param>` tag for each parameter describing what *effect* it has (rather than what it *is*).

However, prefer to give each parameter a really communicative name. If those names end up covering all the user could need to know about *all* the parameters, remove all the `<param>` tags to avoid super redundant documentation. If they do not, add relevant details to each `<param>` to the extent possible.

The same applies to `<typeparam>` tags. They are shown by intellisense as the user enters type parameter lists exactly the way `<param>` descriptions are shown as the user enters method parameter lists. `<typeparam>` elements are just as important on generic methods as they are on generic type definitions.


### `<returns>` and `<value>`

Don't spend any time on the `<returns>` or `<value>` tags since the contents are not typically seen. Important details should all be in the `<summary>`. If the IDE auto-inserts them, just remove them.


### Empty tags

Do not leave any empty tags. Either remove them or fill them out. This includes the compiler's all-or-none `<param>` and `<typeparam>` tag enforcement.


### `<exception>`

Consider documenting things that lead to exceptions being thrown using the `<exception>` tag.
This can really get the end user up to speed on things that aren't immediately obvious from the method signature.

Sadly intellisense will only show the `<exception>` type thrown, not the message, so for important messages it's good to also include the message a second time in the summary:
```csharp
/// <summary>
/// Does foo. If <see cref="OtherProperty"/> is not set, throws <see cref="InvalidOperationException"/>.
/// </summary>
/// <exception cref="InvalidOperationException">Thrown when <see cref="OtherProperty"/> is not set.</exception>
```