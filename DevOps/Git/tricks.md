1. Interactive Add
ممكن تضيف جزء معين من الملف للـ commit بدل ما تضيف الملف كله.
git add -p
استخدمها لو عايز تعزل التعديلات الخاصة بكل حاجة لو بتشتغل على مميزات مختلفة في نفس الملف.

2. Undo the Last Commit
لو عملت commit بالغلط، تقدر تلغي آخر commit بدون ما تفقد التعديلات.
git reset --soft HEAD~1
ممكن تستخدم `--hard` لو عايز تلغي كل حاجة.

3. Check Your Branch's Upstream Status
ده بيخلي Git يتأكد من كل التحديثات على الـ remote ويمسح الفروع اللي اتشالت.
git fetch --all --prune
ده مهم علشان تفضل بيئة العمل عندك نظيفة.

4. Quick Commit Fix
لو نسيت تضيف ملف أو عملت typo في الـ commit message، تقدر تصلح ده بسرعة.
git commit --amend
استخدمها علشان تصلح الأخطاء بدون ما تضيف commit جديد.

5. Stash Your Work
لو عايز تسيب التعديلات وتغير الفرع من غير ما تعمل commit.
git stash
لو عايز تضيف تعليق على الـ stash: `git stash save "description".

6. Pop Your Stash
لما تكون جاهز ترجعلها تاني، استخدم الأمر ده.
git stash pop
لو مش عايز تمسح الـ stash بعد التطبيق: `git stash apply`.

7. Cherry-Picking Commits
لو عايز تدمج commit معين من فرع تاني بدون دمج الفرع كله.
git cherry-pick <commit-hash>
ده مهم لو عايز تجيب fix صغير من فرع تاني.

8. Clean Up Local Branches
بعد ما تخلص من ميزة، لو عايز تمسح الفرع المحلي.
git branch -d <branch-na
لو الفرع لسه مش متدمج: `git branch -D <branch-name>`.

9. View File History
علشان تشوف كل التعديلات على ملف معين.
git log -- <file>
ده بيساعدك تتابع التعديلات في المشاريع المشتركة.

10. Blame a Line of Code
لو عايز تعرف مين كتب سطر معين من الكود.
git blame <filename>
تقدر تستخدم ده في الـ debugging.

11. Find the Source of a Bug
لو عندك bug مش قادر تحدد مكانه، Git هيعمل بحث ثنائي علشان يلاقي commit اللي عمل المشكلة.
git bisect start
git bisect bad
git bisect good <older-commit-hash>
استخدمه لما يكون عندك bug مش قادر تلاقيه بسهولة.

12. Abort a Merge
لو الميرج مش ماشي صح، تقدر تلغي الميرج وتعود لحالتك السابقة.
git merge --abort
دايمًا تأكد إن البيئة عندك نظيفة قبل ما تبدأ الميرج.

13. Search Commit Messages
لو بتدور على commit معين أو كلمة معينة في الرسائل.
git log --grep="search term"
استخدمه علشان تلاقي commit بسرعة لو مش فاكر الـ hash.

14. Tagging a Commit
لو عايز تحدد نقطة معينة في تاريخ المشروع، زي إصدار.
git tag -a v1.0 -m "Version 1.0 release"
استخدم tags علشان تقدر ترجع للإصدارات بسهولة.

15. Hard Reset to Clean the Workspace
لو عايز تمسح الملفات غير المتابعة وتبدأ من جديد.
git clean -fd
استخدمه بحذر علشان لو كنت محتاج الملفات دي بعدين.