### 📄 **Feature Specification: *Fragments* Section**

---

#### **Overview**

The **Fragments** section is a dynamic, exploratory content area intended to serve as a public-facing *creative sketchbook* for Media Nocta. It will host diverse microcontent entries — referred to as “fragments” — that are too brief, raw, or abstract for the blog or project sections. This feature encourages non-linear exploration and conveys the intellectual and artistic texture of the studio.

---

#### **Purpose**

* Provide a structured, low-friction outlet for short-form creative or technical ideas.
* Highlight the creative process behind larger projects and philosophies.
* Express the studio's interdisciplinary nature through fragments of thought, code, fiction, and design.
* Encourage recurring engagement through a growing archive of nontraditional content.

---

#### **Core Content Model**

A prototypal model for a Fragment looks as follow : 

| Field        | Type                 | Description                                                                                                         |
|--------------|----------------------|---------------------------------------------------------------------------------------------------------------------|
| `title`      | string               | Optional poetic or functional title.                                                                                |
| `content`    | string / rich text   | Textual body of the fragment                                                                                        |
| `attachment` | image / video / code | Additionnal content like code, image, video...                                                                      |
| `type_tag`   | enum                 | Tag defining the fragment category (e.g., `idea`, `code`, `quote`, `glitch`, `dream`, `fragment`, `fiction`, etc.). |
| `date`       | date                 | Creation or publication date of the fragment.                                                                       |
| `meta`       | object               | Optional notes, references, hashtags, or external links.                                                            |
| `vibe_count` | int                  | (Optional) Anonymous "likes" or signal metric (non-personalized).                                                   |
| `public`     | bool                 | Whether the fragment should be displayed pubicly on the website or stay hidden.                                     |

---

#### **Interaction Requirements**

* **Browsing Interface**
  
  * Masonry-style list of fragments on the page
  * Filtering by tag, search bar at the top.

* **Fragment Card / Tile Component**

  * Displays `title`, `type_tag`, truncated content (or full if short).
  * Optional iconography or styling based on tag.
  * Click to expand into full view if fragment is longer or has rich content.

* **Resonance Interaction (Optional)**

  * Fragments may include a ✦ icon allowing visitors to “resonate” anonymously.
  * Resonance is a simple integer counter; no login, no identity stored.
  * Optional tooltip: “This hit me.”

---

#### **Visual / UX Design Goals**

* Maintain a style distinct from blog/project sections.
* Emphasize rawness, expressiveness, and non-linearity.
* Tone may be minimalist (e.g. codex-style) or experimental (e.g. brutalist, sketchbook).
* Theme must reflect Media Nocta’s dark, futuristic aesthetic.

---

#### **Implementation Notes**

* Fragments are stored as database entries.
* Support for syntax highlighting (if `code` fragments).
* Optional image support for `glitch`, `sketch`, or `visual` tags.
* SEO: Optional schema markup (`CreativeWork`, `Note`, etc.).

---

## ✍️ **Example Entries**

* **🧠 Title**: *Echo of Thought*\
**Tag**: `idea`\
**Date**: 2025-07-04\
**Content**:

  > A commenting system where each comment fades over time unless re-echoed by another reader. Memory as a democratic filter.



* **🧪 Title**: *The One-Time Hook*\
**Tag**: `code` \
**Date**: 2025-06-17\
**Content**:

```ts
useEffect(() => {
  console.log("First and only time.");
  return () => {}; // No cleanup
}, []);
```

---

* **🌫️ Title**: *Unnamed Error*\
**Tag**: `glitch`\
**Date**: 2025-06-01\
**Content**:\
Screenshot of your game showing an NPC sinking infinitely into the floor.\
**Meta**:\
“This happened after adding slope detection. Never fixed it. Maybe I shouldn’t.”

---

#### **Future Extension Ideas (non-MVP)**

* “Fragment Series” for grouped or evolving ideas.
* Attach fragments to blog entries or tech projects.
* Visitor-submitted fragments (moderated).
* Time-based decay (fragments fade from view unless re-engaged).
* RSS feed for fragments.

---







