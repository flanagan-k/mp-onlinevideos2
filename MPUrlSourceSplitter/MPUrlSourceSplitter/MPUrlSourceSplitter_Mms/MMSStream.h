/*
    Copyright (C) 2007-2010 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2.  If not, see <http://www.gnu.org/licenses/>.
*/

#pragma once

#ifndef __MMSSTREAM_DEFINED
#define __MMSSTREAM_DEFINED

#include "MPUrlSourceSplitter_Mms_Exports.h"

class MPURLSOURCESPLITTER_MMS_API MMSStream
{
public:
  // constructor
  // create instance of MMSStream class
  MMSStream(void);

  // destructor
  ~MMSStream(void);

  // sets stream id
  // @param id : the stream id
  void SetId(int id);

  // gets stream id
  // @return : stream id
  int GetId(void);

protected:
  int id;
};

#endif