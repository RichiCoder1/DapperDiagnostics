#Dapper Diagnostics
Dapper diagnostics is a set of roslyn-powered diagnostics that assist in sanity checking dapper calls.

This includes ensuring:
* Parameters mentioned in the SQL Text are in the `params` object. *Warning*
* All properties in the `params` arugment are featured in the SQL text. *Hint*
* The select statement matches up (roughly) with the return object. *Warning*

These checks will also only be applied if the SQL text is constant. If it's generated or variable, the diagnostic will simply bail out.

#Build Status

[![dapperdiagnostics MyGet Build Status](https://www.myget.org/BuildSource/Badge/dapperdiagnostics?identifier=cbbc0b3f-88a7-44fe-8c63-4d54aa8adf49)](https://www.myget.org/)

#How To Get

Nuget Package Feed: https://www.myget.org/F/dapperdiagnostics/api/v2

Visx Feed: https://www.myget.org/F/dapperdiagnostics/api/v2

Thanks MyGet for being awesome!

#Notes

1. This is a personal project. I have no intentions to maintain, expand, or extend it. I'm open sourcing it largely as an example piece of code.
2. It only works with oracle. If you're saying `wat`, I completely agree. That, however, is what I use currently where this matters.